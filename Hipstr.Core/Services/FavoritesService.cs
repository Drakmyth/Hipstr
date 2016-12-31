using Hipstr.Core.Models;
using System.Collections.Generic;

namespace Hipstr.Core.Services
{
	public interface IFavoritesService
	{
		// TODO: Consider an IFavoritable interface?
		void MarkFavorite(Room room);
		void MarkFavorite(User user);
		void UnmarkFavorite(Room room);
		void UnmarkFavorite(User user);
	}

	public class FavoritesService : IFavoritesService
	{
		private readonly List<Room> _favoriteRooms;
		private readonly List<User> _favoriteUsers;

		public FavoritesService()
		{
			_favoriteRooms = new List<Room>();
			_favoriteUsers = new List<User>();
		}

		public void MarkFavorite(Room room)
		{
			_favoriteRooms.Add(room);
		}

		public void MarkFavorite(User user)
		{
			_favoriteUsers.Add(user);
		}

		public void UnmarkFavorite(Room room)
		{
			_favoriteRooms.Remove(room);
		}

		public void UnmarkFavorite(User user)
		{
			_favoriteUsers.Remove(user);
		}
	}
}