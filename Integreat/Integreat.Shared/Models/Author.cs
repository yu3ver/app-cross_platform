﻿using Newtonsoft.Json;
using SQLite;

namespace Integreat.Shared.Models
{
	[Table ("Author")]
	public class Author
	{
		[PrimaryKey]
		[JsonProperty ("login")]
		public string Login{ get; set; }

		[JsonProperty ("first_name")]
		public string FirstName{ get; set; }

		[JsonProperty ("last_name")]
		public string LastName{ get; set; }

		public Author (string login, string firstName, string lastName)
		{
			Login = login;
			FirstName = firstName;
			LastName = lastName;
		}

		public Author ()
		{
		}
	}
}

