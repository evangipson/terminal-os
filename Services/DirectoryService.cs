﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

using Terminal.Constants;
using Terminal.Enums;
using Terminal.Extensions;
using Terminal.Models;

namespace Terminal.Services
{
    /// <summary>
    /// A global singleton that is responsible for getting and setting directory values.
    /// </summary>
    public partial class DirectoryService : Node
    {
        /// <summary>
        /// The <see cref="Models.FileSystem"/> of the console.
        /// </summary>
        public FileSystem FileSystem;

        public override void _Ready()
        {
            FileSystem = new()
            {
                Directories = DirectoryConstants.GetDefaultDirectoryStructure()
            };

            SetCurrentDirectory(GetHomeDirectory());
        }

        /// <summary>
        /// Sets the current directory of the <see cref="FileSystem"/> to the provided <paramref name="newCurrentDirectory"/>.
        /// <para>
        /// Does nothing if the provided <paramref name="newCurrentDirectory"/> can't be found.
        /// </para>
        /// </summary>
        /// <param name="newCurrentDirectory">
        /// The new directory to use as the current directory.
        /// </param>
        public void SetCurrentDirectory(DirectoryEntity newCurrentDirectory)
        {
            if (newCurrentDirectory?.IsDirectory != true || FileSystem == null)
            {
                return;
            }

            FileSystem.CurrentDirectoryId = newCurrentDirectory.Id;
        }

        /// <summary>
        /// Sets the current directory of the <see cref="FileSystem"/> to the provided <paramref name="newDirectoryPath"/>.
        /// <para>
        /// Does nothing if the provided <paramref name="newDirectoryPath"/> can't be resolved.
        /// </para>
        /// </summary>
        /// <param name="newDirectoryPath">
        /// The new directory path to parse and use as the current directory.
        /// </param>
        public void SetCurrentDirectory(string newDirectoryPath)
        {
            if (string.IsNullOrEmpty(newDirectoryPath))
            {
                return;
            }

            if (newDirectoryPath == "/")
            {
                SetCurrentDirectory(GetRootDirectory());
            }

            List<string> directoryTokensInPath = newDirectoryPath.Split('/').ToList();
            DirectoryEntity newCurrentDirectory = GetCurrentDirectory().FindDirectory(directoryTokensInPath.LastOrDefault().TrimEnd('/'));
            newCurrentDirectory ??= GetRootDirectory().FindDirectory(newDirectoryPath.TrimEnd('/'));

            SetCurrentDirectory(newCurrentDirectory);
        }

        /// <summary>
        /// Gets the parent directory of the provided <paramref name="currentDirectory"/>.
        /// <para>
        /// Gets the root directory if a parent directory is not found.
        /// </para>
        /// </summary>
        /// <param name="currentDirectory">
        /// The file or folder to get the parent of.
        /// </param>
        /// <returns>
        /// The parent directory of the <paramref name="currentDirectory"/>.
        /// </returns>
        public DirectoryEntity GetParentDirectory(DirectoryEntity currentDirectory) => GetRootDirectory().FindDirectory(currentDirectory.ParentId) ?? GetRootDirectory();

        /// <summary>
        /// Gets the root directory of the <see cref="FileSystem"/>.
        /// <para>
        /// Defaults to <see langword="null"/> if the <see cref="FileSystem"/> is <see langword="null"/>.
        /// </para>
        /// </summary>
        /// <returns>
        /// The root directory of the <see cref="FileSystem"/>.
        /// </returns>
        public DirectoryEntity GetRootDirectory() => FileSystem?.Root;

        /// <summary>
        /// Gets the current directory of the <see cref="FileSystem"/>.
        /// <para>
        /// If the current directory isn't found, gets the root directory.
        /// </para>
        /// </summary>
        /// <returns>
        /// The current directory of the <see cref="FileSystem"/>.
        /// </returns>
        public DirectoryEntity GetCurrentDirectory() => GetRootDirectory().FindDirectory(FileSystem?.CurrentDirectoryId ?? Guid.Empty) ?? GetRootDirectory();

        /// <summary>
        /// Gets the absolute path of the current directory.
        /// <para>
        /// Defaults to <see langword="null"/> if the <see cref="FileSystem"/> is <see langword="null"/>.
        /// </para>
        /// </summary>
        /// <returns>
        /// The absolute path of the current directory of the <see cref="FileSystem"/>.
        /// </returns>
        public string GetCurrentDirectoryPath() => FileSystem?.GetDirectoryPath(GetCurrentDirectory());

        /// <summary>
        /// Gets an absolute path for the provided <paramref name="directory"/> folder.
        /// <para>
        /// Defaults to <see langword="null"/> if the <see cref="FileSystem"/> is <see langword="null"/>.
        /// </para>
        /// </summary>
        /// <param name="directory">
        /// The folder to get the absolute directory path for.
        /// </param>
        /// <returns>
        /// An absolute path of the provided <paramref name="directory"/> folder.
        /// </returns>
        public string GetAbsoluteDirectoryPath(DirectoryEntity directory) => FileSystem?.GetDirectoryPath(directory);

        /// <summary>
        /// Gets an absolute path for the provided <paramref name="entity"/> file or folder.
        /// <para>
        /// Defaults to <see langword="null"/> if the <see cref="FileSystem"/> is <see langword="null"/>.
        /// </para>
        /// </summary>
        /// <param name="entity">
        /// The file or folder to get the absolute path for.
        /// </param>
        /// <returns>
        /// An absolute path of the provided <paramref name="entity"/> file or folder.
        /// </returns>
        public string GetAbsoluteEntityPath(DirectoryEntity entity) => FileSystem?.GetEntityPath(entity);

        /// <summary>
        /// Gets a relative path for the provided <paramref name="entity"/> file or folder.
        /// <para>
        /// Defaults to <see langword="null"/> if the <see cref="FileSystem"/> is <see langword="null"/>.
        /// </para>
        /// </summary>
        /// <param name="entity">
        /// The file or folder to get the relative path for.
        /// </param>
        /// <returns>
        /// A relative path of the provided <paramref name="entity"/> file or folder.
        /// </returns>
        public string GetRelativeEntityPath(DirectoryEntity entity) => FileSystem?.GetEntityPath(entity).Replace(GetCurrentDirectoryPath(), string.Empty);

        /// <summary>
        /// Gets a file with the provided <paramref name="fileName"/> from the current directory.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to get.
        /// </param>
        /// <returns>
        /// A file with the provided <paramref name="fileName"/> from the current directory.
        /// </returns>
        public DirectoryEntity GetRelativeFile(string fileName) => GetCurrentDirectory().FindFile(fileName);

        /// <summary>
        /// Gets a file with the provided <paramref name="fileName"/> from the root directory.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to get.
        /// </param>
        /// <returns>
        /// A file with the provided <paramref name="fileName"/> from the root directory.
        /// </returns>
        public DirectoryEntity GetAbsoluteFile(string fileName) => GetRootDirectory().FindFile(fileName);

        /// <summary>
        /// Gets a folder with the provided <paramref name="directoryName"/> from the current directory.
        /// </summary>
        /// <param name="directoryName">
        /// The name of the folder to get.
        /// </param>
        /// <returns>
        /// A folder with the provided <paramref name="directoryName"/> from the current directory.
        /// </returns>
        public DirectoryEntity GetRelativeDirectory(string directoryName) => GetCurrentDirectory().FindDirectory(directoryName.TrimEnd('/'));

        /// <summary>
        /// Gets a folder with the provided <paramref name="directoryPath"/> from the root directory.
        /// </summary>
        /// <param name="directoryPath">
        /// The name of the folder to get.
        /// </param>
        /// <returns>
        /// A folder with the provided <paramref name="directoryPath"/> from the root directory.
        /// </returns>
        public DirectoryEntity GetAbsoluteDirectory(string directoryPath) => GetRootDirectory().FindDirectory(directoryPath);

        /// <summary>
        /// Gets the home folder for the current user.
        /// <para>
        /// Defaults to the <c>/users/user/home/</c> directory.
        /// </para>
        /// </summary>
        /// <returns>
        /// The home folder folder.
        /// </returns>
        public DirectoryEntity GetHomeDirectory() => GetRootDirectory().FindDirectory("users/user/home");

        /// <summary>
        /// Creates a file with the provided <paramref name="fileName"/> in the current directory.
        /// <para>
        /// Default permissions are <see cref="Permission.UserRead"/> and <see cref="Permission.UserWrite"/>.
        /// </para>
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to create in the current directory.
        /// </param>
        public void CreateFile(string fileName)
        {
            var newFile = new DirectoryFile()
            {
                ParentId = GetCurrentDirectory().Id,
                Permissions = new() { Permission.UserRead, Permission.UserWrite }
            };

            var name = fileName;
            var fileTokens = fileName.Split('.');
            if (fileTokens.Length > 1)
            {
                name = string.Join('.', fileTokens.Take(fileTokens.Length - 1));
                newFile.Extension = fileTokens.Last();
            }

            newFile.Name = name;
            GetCurrentDirectory().Entities.Add(newFile);
        }

        /// <summary>
        /// Creates a folder with the provided <paramref name="directoryName"/> in the current directory.
        /// <para>
        /// Default permissions are <see cref="Permission.UserRead"/> and <see cref="Permission.UserWrite"/>.
        /// </para>
        /// </summary>
        /// <param name="directoryName">
        /// The name of the folder to create in the current directory.
        /// </param>
        public void CreateDirectory(string directoryName)
        {
            var newDirectory = new DirectoryFolder()
            {
                Name = directoryName,
                Permissions = new() { Permission.UserRead, Permission.UserWrite },
                ParentId = GetCurrentDirectory().Id
            };

            GetCurrentDirectory().Entities.Add(newDirectory);
        }

        /// <summary>
        /// Removes a <paramref name="entity"/> from the current directory. Does nothing if the <paramref name="entity"/> doesn't exist.
        /// </summary>
        /// <param name="entity">
        /// The entity to delete.
        /// </param>
        public void DeleteEntity(DirectoryEntity entity)
        {
            if(GetCurrentDirectory().Entities.Contains(entity))
            {
                GetCurrentDirectory().Entities.Remove(entity);
            }
        }
    }
}
