using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace CrashTest.Properties
{
    internal class TestButton : Button
    {
        protected override async void OnClick()
        {

            //queued task open database
            var result = await QueuedTask.Run(() =>
            {
                var filter = new QueryFilter
                {
                    SubFields = "Shape",
                    PrefixClause = "TOP 1",
                    WhereClause = $"ObjectId = 15735",
                    OutputSpatialReference = MapView.Active.Map.SpatialReference
                };

                //make connection file reference
                var connectionFile = new DatabaseConnectionFile(
                    new Uri("c:\\test\\connection.sde"));

                var geodatabase =
                    new Geodatabase(connectionFile);

                var table = geodatabase.OpenDataset<Table>
                    ("Mapworks.SDE.Wire");

                //if the column I want isn't there, bail
                if (table.GetDefinition().FindField("Shape") < 0)
                    return null;
                try
                {
                    using (var read = table.Search(filter, true))
                    {
                        if (read.MoveNext())
                        {
                            return read.Current["Shape"];
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    table.Dispose();
                    geodatabase.Dispose();
                }

                return null;
            });
        }
    }
}
