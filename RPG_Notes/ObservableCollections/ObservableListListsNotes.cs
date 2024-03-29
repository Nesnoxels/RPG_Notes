﻿using RPG_Notes.Models;
using RPG_Notes.Services.DataService;
using RPG_Notes.Services.Source;
using RPG_Notes.UserControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Notes.ObservableCollections;

public class ObservableListListsNotes : IDataService<int, ListNotes>
{
    public ObservableListListsNotes()
    {
        Lists = [];
        FillLists();
    }

    public ObservableCollection<ListNotesView> Lists { get; }

    private readonly ListNotesService _service = new();

    private async void FillLists()
    {
        foreach (var list in await _service.GetAllAsync().ToHashSetAsync())
            Lists.Add(new() { Notes = new(list) });
    }

    public IAsyncEnumerable<ListNotes> GetAllAsync() =>
        _service.GetAllAsync();

    public async Task<ListNotes?> GetAsync(int key) =>
        await _service.GetAsync(key);

    public async Task AddAsync(ListNotes value)
    {
        var task = _service.AddAsync(value);
        Lists.Add(new() { Notes = new(value) });
        await task;
    }

    public async Task AddAsync(string name) =>
        await AddAsync(new ListNotes(((await GetAllAsync().LastOrDefaultAsync())?.Id ?? 0) + 1, name));

    public async Task UpdateAsync(ListNotes value)
    {
        var task = _service.UpdateAsync(value);
        Lists[Lists.IndexOf(Lists.First(l => l.Notes.ListInfo.Id == value.Id))] = new() { Notes = new(value) };
        await task;
    }

    public async Task UpdateAsync(int id, string name) =>
        await UpdateAsync(new(id, name));

    public async Task RemoveAsync(int key)
    {
        var task = _service.RemoveAsync(key);
        Lists.Remove(Lists.First(n => n.Notes.ListInfo.Id == key));
        await task;
    }
}
