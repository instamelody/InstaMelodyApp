using System;
using System.Data;
using InstaMelody.Data;
using InstaMelody.Model;

namespace InstaMelody.Business
{
    public class ImageBLL
    {
        /// <summary>
        /// Adds the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">
        /// Cannot create image with empty File Name.
        /// or
        /// Cannot add new image with existing File Name.
        /// or
        /// Failed to create Image.
        /// </exception>
        public Image AddImage(Image image)
        {
            if (string.IsNullOrWhiteSpace(image.FileName))
            {
                throw new ArgumentException("Cannot create image with empty File Name.");
            }

            if (this.DoesImageExistWithFileName(image.FileName))
            {
                throw new DataException("Cannot add new image with existing File Name.");
            }

            var dal = new Images();
            image.DateCreated = DateTime.UtcNow;
            var createdImage = dal.CreateImage(image);
            if (createdImage == null)
            {
                throw new DataException("Failed to create Image.");
            }

            return createdImage;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.DataException">No Image Id or File name provided</exception>
        public Image GetImage(Image image)
        {
            Image result = null;

            if (image.Id.Equals(default(int)) && string.IsNullOrWhiteSpace(image.FileName))
            {
                throw new ArgumentException("No Image Id or File name provided.");
            }

            var dal = new Images();
            if (!image.Id.Equals(default(int)))
            {
                result = dal.GetImageById(image.Id);
            }
            else
            {
                result = dal.GetImageByFileName(image.FileName);
            }

            return result;
        }

        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <exception cref="System.Data.DataException">
        /// No Image Id provided.
        /// or
        /// Failed to delete image.
        /// </exception>
        public int DeleteImage(Image image)
        {
            var dal = new Images();
            Image foundImage = null;
            if (!image.Id.Equals(default(int)))
            {
                foundImage = dal.GetImageById(image.Id);
            }

            if (foundImage == null)
            {
                foundImage = dal.GetImageByFileName(image.FileName);
            }

            if (foundImage == null)
            {
                throw new ArgumentException("Could not find image to delete.");
            }

            dal.DeleteImage(foundImage.Id);

            var getImage = dal.GetImageById(foundImage.Id);
            if (getImage != null)
            {
                throw new DataException("Failed to delete image.");
            }

            return foundImage.Id;
        }

        /// <summary>
        /// Checks for duplicate records with the name provided.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private bool DoesImageExistWithFileName(string fileName)
        {
            var dal = new Images();
            var existing = dal.GetImageByFileName(fileName);

            return (existing != null);
        }
    }
}
