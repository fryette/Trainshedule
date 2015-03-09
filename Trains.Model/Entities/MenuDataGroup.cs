using System;
using System.Collections.ObjectModel;
using TrainSearch.Entities;

namespace Trains.Model.Entities
{
    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class MenuDataGroup
    {
        public MenuDataGroup(String uniqueId, String title)
        {
            UniqueId = uniqueId;
            Title = title;
            Items = new ObservableCollection<MenuDataItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public ObservableCollection<MenuDataItem> Items { get; private set; }

        public override string ToString()
        {
            return Title;
        }
    }
}