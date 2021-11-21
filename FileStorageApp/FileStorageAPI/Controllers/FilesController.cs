﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// API информации о файлах из Telegram.
    /// </summary>
    [Route("api/files")]
    [SwaggerTag("Информация об файлах из Telegram")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// Инициализирует новый экземляр класса <see cref="FilesController"/>.
        /// </summary>
        /// <param name="fileService">Сервис для взаимодействия с информацией о файлах</param>
        public FilesController(IFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        /// <summary>
        /// Возвращает список файлов, к которым пользователь имеет доступ.
        /// </summary>
        /// <param name="fileSearchParameters">Параметры поиска файлов</param>
        /// <param name="skip">Количество пропускаемых элементов</param>
        /// <param name="take">Количество возвращаемых элементов</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает все доступные файлы для текущего пользователя", typeof(FileInfo))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Если skip или take меньше 0", typeof(string))]
        public async Task<IActionResult> GetFileInfos([FromQuery] FileSearchParameters fileSearchParameters, [FromQuery, Required] int skip, [FromQuery, Required] int take)
        {
            var files = await _fileService.GetFileInfosAsync(fileSearchParameters, skip, take);

            return files.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(files.Value),
                HttpStatusCode.BadRequest => BadRequest(files.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Возвращает информацию о файле по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает информацию о файле по заданному идентификатору", typeof(FileInfo))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если файл с таким идентификатором не найден", typeof(string))]
        public async Task<IActionResult> GetFileInfoById(Guid id)
        {
            var file = await _fileService.GetFileInfoByIdAsync(id);

            return file.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(file.Value),
                HttpStatusCode.NotFound => NotFound(file.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Возвращает ссылку для скачивания файла.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet("{id:guid}/downloadlink")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает ссылку для скачивания файла по заданному идентификатору", typeof(FileInfo))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если файл с таким идентификатором не найден", typeof(string))]
        public async Task<IActionResult> GetFileDownloadLink(Guid id)
        {
            var file = await _fileService.GetFileDownloadLinkByIdAsync(id);

            return file.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(file.Value),
                HttpStatusCode.NotFound => NotFound(file.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Добавление файла в хранилище.
        /// </summary>
        /// <param name="file">Файл</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Возвращает информацию о созданном файле", typeof(FileInfo))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Может выкинуться, если что-то не так с бд")]
        public async Task<IActionResult> PostFile([FromForm]IFormFile file)
        {
            var uploadedFile = await _fileService.CreateFileAsync(file);

            return uploadedFile.ResponseCode switch
            {
                HttpStatusCode.Created => Created(uploadedFile.Value.Uri, uploadedFile.Value.Info),
                HttpStatusCode.InternalServerError => StatusCode(500, "Something wrong with database"),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Обновление названия файла.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="fileName">Новое имя файла</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpPut("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status201Created, "Возвращает информацию об обновленном файле", typeof(FileInfo))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если файл с таким идентификатором не найден", typeof(string))]
        public async Task<IActionResult> PutFile(Guid id, [FromBody]UpdateFile fileName)
        {
            var file = await _fileService.UpdateFileAsync(id, fileName.FileName);

            return file.ResponseCode switch
            {
                HttpStatusCode.Created => Created(file.Value.Uri, file.Value.Info),
                HttpStatusCode.NotFound => NotFound(file.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Удаление файла.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpDelete("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Возвращает информацию об удаленном файле", typeof(FileInfo))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если файл с таким идентификатором не найден", typeof(string))]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var file = await _fileService.DeleteFileAsync(id);

            return file.ResponseCode switch
            {
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.NotFound => NotFound(file.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}