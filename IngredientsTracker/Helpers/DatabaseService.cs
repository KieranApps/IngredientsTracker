using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace IngredientsTracker.Helpers
{
    public class DatabaseService
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");

    }
}
