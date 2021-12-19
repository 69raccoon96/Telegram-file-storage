﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.FileSenders
{
    internal class FileSenderStorage : BaseStorage<FileSender>, IFileSenderStorage
    {
        internal FileSenderStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
            Database.EnsureCreated();
        }

        public Task<List<FileSender>> GetAllAsync()
        {
            return DbSet
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<List<FileSender>> GetBySenderNameSubstringAsync(string subString)
        {
            if (subString == null)
                throw new ArgumentNullException(nameof(subString));

            return DbSet
                .Where(x => x.FullName.Contains(subString))
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<List<FileSender>> GetByTelegramNameSubstringAsync(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));

            return DbSet
                .Where(x => x.TelegramUserName.Contains(userName))
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public new Task<FileSender> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        public Task<bool> ContainsByTelegramIdAsync(long id)
        {
            return DbSet.AnyAsync(sender => sender.TelegramId == id);
        }
    }
}