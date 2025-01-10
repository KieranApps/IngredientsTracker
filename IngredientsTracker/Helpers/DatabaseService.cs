using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngredientsTracker.Helpers
{
    public class DatabaseService
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");

    }
}
