using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace DevionGames.StatSystem.Configuration
{
    [System.Serializable]
    public class StatSettingsEditor : ScriptableObjectCollectionEditor<Settings>
    {
        [SerializeField]
        protected List<string> searchFilters;
        [SerializeField]
        protected string searchFilter = "All";


        public override string ToolbarName
        {
            get
            {
                return "Settings";
            }
        }

        protected override bool CanAdd
        {
            get
            {
                return false;
            }
        }

        protected override bool CanRemove
        {
            get
            {
                return false;
            }
        }

        public StatSettingsEditor(UnityEngine.Object target, List<Settings> items, List<string> searchFilters) : base(target, items)
        {
            this.target = target;
            this.items = items;
            this.searchFilters = searchFilters;
            this.searchFilters.Insert(0, "All");
            this.m_SearchString = "All";

            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(Settings).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract).ToArray();

            foreach (Type type in types)
            {
                if (Items.Where(x => x.GetType() == type).FirstOrDefault() == null)
                {
                    CreateItem(type);
                }
            }
        }

        protected override void DoSearchGUI()
        {
            string[] searchResult = EditorTools.SearchField(this.m_SearchString, searchFilter, searchFilters, GUILayout.Width(m_SidebarRect.width - 20));
            searchFilter = searchResult[0];
            m_SearchString = searchResult[1];
        }

        protected override bool MatchesSearch(Settings item, string search)
        {
            return true;//(item.Name.ToLower().Contains(search.ToLower()) || searchString == searchFilter || search.ToLower() == item.GetType().Name.ToLower()) && (searchFilter == "All" || item.Category.Name == searchFilter);
        }

        protected override string ButtonLabel(int index, Settings item)
        {
            return "  " + GetSidebarLabel(item);
        }
    }
}