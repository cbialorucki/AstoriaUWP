using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace dex.net
{
	public class WritersFactory
	{
		private Dictionary<string,IDexWriter> _writers;


        //Does anything even use this in this library?

        //This is a list of different DexWriters

        /*public async Task<WritersFactory> CreateAsync()
        {
            WritersFactory wf = new WritersFactory();
            var assemblyList = await GetAssemblyList();
            IEnumerable<Assembly> writers = assemblyList.SelectMany(s => s.GetTypes()).Where(p => typeof(IDexWriter).IsAssignableFrom(p));

            wf._writers = new Dictionary<string, IDexWriter>();
            foreach (Assembly writer in writers)
            {
                var writerInstace = (IDexWriter)Activator.CreateInstance(writer);
                wf._writers.Add(writerInstace.GetName(), writerInstace);
            }

            return wf;
        }*/

        //private WritersFactory() { }

        /*public WritersFactory()
        {
            List<IDexWriter> writers = new List<IDexWriter>();
        }*/

		/*public WritersFactory ()
		{
            //AppContext.BaseDirectory.Ge
			//var writers = AppDomain.CurrentDomain.GetAssemblies()
						//.SelectMany(s => s.GetTypes())
					//.Where(p => typeof(IDexWriter).IsAssignableFrom(p) && p.IsClass);
                    var writers = GetAssemblyList()

			_writers = new Dictionary<string,IDexWriter> ();
			foreach(var writer in writers) {
				var writerInstace = (IDexWriter)Activator.CreateInstance (writer);
				_writers.Add (writerInstace.GetName(), writerInstace);
			}*/
		//}

		public string[] GetWriters()
		{
			var names = new string[_writers.Keys.Count];
			_writers.Keys.CopyTo (names, 0);

			return names;
		}

		public IDexWriter GetWriter(string name)
		{
			return _writers [name];
		}

        public static async Task<List<Assembly>> GetAssemblyList()
        {
            List<Assembly> assemblies = new List<Assembly>();

            var files = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync();
            if (files == null)
                return assemblies;

            foreach (var file in files.Where(file => file.FileType == ".dll" || file.FileType == ".exe"))
            {
                try
                {
                    assemblies.Add(Assembly.Load(new AssemblyName(file.DisplayName)));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

            }

            return assemblies;
        }
    }
}