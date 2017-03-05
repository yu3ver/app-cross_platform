﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Integreat.Shared.Models;
using SQLiteNetExtensionsAsync.Extensions;

namespace Integreat.Shared.Services.Persistence
{
    public partial class PersistenceService
    {



        public Task<List<T>> GetPages<T>(Language language, string parentPage) where T : Page, new() {
            if (parentPage == null)
            {
                return Connection.GetAllWithChildrenAsync<T>(x => x.LanguageId == language.PrimaryKey &&
                                                                  !"trash".Equals(x.Status))
                    .DefaultIfFaulted(new List<T>());
            }
            return Connection.GetAllWithChildrenAsync<T>(x => x.LanguageId == language.PrimaryKey &&
                                                              !"trash".Equals(x.Status) &&
                                                              // if a parent-page is set, we only return pages with this parent-id
                                                              x.ParentId == parentPage).DefaultIfFaulted(new List<T>());
        }

        public Task<int> CountPages<T>(Language language) where T : Page, new()
        {
            return Connection.Table<T>().Where(x => x.LanguageId == language.PrimaryKey).CountAsync().DefaultIfFaulted();
        }
    }
}
