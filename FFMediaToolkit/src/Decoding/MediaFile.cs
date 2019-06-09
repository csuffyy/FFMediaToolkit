﻿namespace FFMediaToolkit.Decoding
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using FFMediaToolkit.Decoding.Internal;

    /// <summary>
    /// Represents a multimedia file.
    /// </summary>
    public class MediaFile : IDisposable
    {
        private readonly InputContainer container;
        private bool isDisposed;

        private MediaFile(InputContainer container, MediaOptions options)
        {
            this.container = container;

            if (container.Video != null)
            {
                Video = new VideoStream(container.Video, options);
            }
        }

        /// <summary>
        /// Gets the video stream.
        /// </summary>
        public VideoStream Video { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the file contains video stream and the stream is loaded.
        /// </summary>
        public bool HasVideo => Video != null;

        /// <summary>
        /// Opens a media file from the specified path.
        /// </summary>
        /// <param name="path">A path to the media file.</param>
        /// <param name="options">The decoder settings.</param>
        /// <returns>The opened <see cref="MediaFile"/>.</returns>
        public static MediaFile Open(string path, MediaOptions options)
        {
            try
            {
                var container = InputContainer.LoadFile(path, options);
                return new MediaFile(container, options);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (DirectoryNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to open the media file", ex);
            }
        }

        /// <summary>
        /// Opens a media file from the specified path asynchronously.
        /// </summary>
        /// <param name="path">A path to the media file.</param>
        /// <param name="options">The decoder settings.</param>
        /// <returns>The opened <see cref="MediaFile"/>.</returns>
        public static async Task<MediaFile> OpenAsync(string path, MediaOptions options)
            => await Task.Run(() => Open(path, options));

        /// <inheritdoc/>
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            if (HasVideo)
            {
                ((IDisposable)Video).Dispose();
                Video = null;
            }

            container.Dispose();

            isDisposed = true;
        }
    }
}
