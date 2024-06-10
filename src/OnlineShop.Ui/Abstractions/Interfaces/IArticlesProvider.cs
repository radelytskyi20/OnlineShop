﻿using OnlineShop.Ui.Models;

namespace OnlineShop.Ui.Abstractions.Interfaces
{
    public interface IArticlesProvider
    {
        Task<IEnumerable<Article>> GetArticles();
    }
}
