﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для взаимодействия с информацией о файлах.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Возвращает весь список файлов.
        /// </summary>
        Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(FileSearchParameters fileSearchParameters, int skip,
            int take, HttpRequest request);

        /// <summary>
        /// Возвращает информацию о файле по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="request"></param>
        Task<RequestResult<FileInfo>> GetFileInfoByIdAsync(Guid id, HttpRequest request);

        /// <summary>
        /// Возвращает ссылку для скачивания файла.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="request"></param>
        Task<RequestResult<string>> GetFileDownloadLinkByIdAsync(Guid id, HttpRequest request);

        /// <summary>
        /// Создает файл, сохраняет его в хранилище и возвращает информацию о нем.
        /// </summary>
        /// <param name="model">Файл</param>
        /// <param name="request"></param>
        Task<RequestResult<(string Uri, FileInfo Info)>> CreateFileAsync(IFormFile model, HttpRequest request);

        /// <summary>
        /// Обновляет имя файла и возвращает информацию о нем.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="fileName">Имя файла</param>
        Task<RequestResult<(string Uri, FileInfo Info)>> UpdateFileAsync(Guid id, string fileName);

        /// <summary>
        /// Удаляет чат и возвращает информацию об удаленном файле.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        Task<RequestResult<FileInfo>> DeleteFileAsync(Guid id);

        /// <summary>
        /// Возвращает количество файлов, содержащихся в хранилище.
        /// </summary>
        Task<RequestResult<int>> GetFilesCountAsync(FileSearchParameters fileSearchParameters, HttpRequest request);

        /// <summary>
        /// Возвращает список названий файлов.
        /// </summary>
        Task<RequestResult<List<string>>> GetFileNamesAsync(HttpRequest request);

        /// <summary>
        /// Возвращает список типов файлов.
        /// </summary>
        RequestResult<FileTypeDescription[]> GetFilesTypes();
    }
}