using System.IO;
using System.Text.Json;
using PlaziatCore;

namespace PlaziatWpf.Services
{

    public class SessionService
    {
		private string name;

		public string User
		{
			get { return name; }
			set { name = value; }
		}

	}

}