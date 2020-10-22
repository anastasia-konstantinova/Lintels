#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace SimpleLintelSetting
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            FamilySymbol familySymbol = new FilteredElementCollector(doc).WhereElementIsElementType().ToElements()
                    .Where(x => x.Name.Equals("5œ¡25-27")).First() as FamilySymbol;

            FilteredElementCollector filteredElementCollector2 = new FilteredElementCollector(doc);
            Level level = filteredElementCollector2.OfClass(typeof(Level)).OfType<Level>().First();

            //FilteredElementCollector filteredElementCollector3 = new FilteredElementCollector(doc);
            //Wall wall = filteredElementCollector3.OfClass(typeof(Wall)).OfType<Wall>().FirstOrDefault();

            try
            {
                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                if (pickedObj != null)
                {
                    
                    ElementId elementId = pickedObj.ElementId;
                    Element element = doc.GetElement(elementId);

                    FamilyInstance fi = element as FamilyInstance;
                    FamilySymbol fs = fi.Symbol;

                    using (Transaction transaction = new Transaction(doc))
                    {
                        transaction.Start("set lintel");

                        /////////////////////
                        Wall wallhost = fi.Host as Wall;
                        //WallType wallType = wallhost.WallType;
                        //double width = wallType.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble();
                        ////////////////////
                        BoundingBoxXYZ boundingBox = wallhost.get_BoundingBox(doc.ActiveView);
                        double y = (boundingBox.Max.Y + boundingBox.Min.Y) * 0.5;
                        /*double width = fs.get_Parameter(BuiltInParameter.WINDOW_WIDTH).AsDouble();
                        double centerWin = width / 2;

                        BoundingBoxXYZ boundingBox = wall.get_BoundingBox(doc.ActiveView);
                        double y = (boundingBox.Max.Y + boundingBox.Min.Y) * 0.5;

                        familySymbol.Activate();
                        Element element2 = familySymbol as Element;
                        doc.Regenerate();

                        BoundingBoxXYZ boundingBox1 = element.get_BoundingBox(doc.ActiveView);
                        double z = boundingBox1.Max.Z;
                        //XYZ xYZ = new XYZ((boundingBox1.Max.X + boundingBox1.Min.X) * 0.5, y, z);
                        XYZ xYZ = new XYZ((boundingBox1.Max.X + boundingBox1.Min.X) * 0.5, centerWin, z);
                        FamilyInstance familyInstance = doc.Create.NewFamilyInstance(xYZ, familySymbol, level, StructuralType.Beam);//
                        //Element element4 = familyInstance as Element;
                        //doc.Regenerate();*/
                        familySymbol.Activate();
                        Element el = familySymbol as Element;
                        doc.Regenerate();

                        BoundingBoxXYZ boundingBoxXYZ = el.get_BoundingBox(doc.ActiveView);
                        double num = (boundingBoxXYZ.Max.Y + boundingBoxXYZ.Min.Y) / 0.5;

                        BoundingBoxXYZ boundingBox1 = element.get_BoundingBox(doc.ActiveView);
                        double z = boundingBox1.Max.Z;
                        XYZ xYZ = new XYZ((boundingBox1.Max.X + boundingBox1.Min.X) * 0.5, y - num/ 4, z);
                        FamilyInstance familyInstance = doc.Create.NewFamilyInstance(xYZ, familySymbol, level, StructuralType.Beam);
                        Element element4 = familyInstance as Element;
                        
                        transaction.Commit();
                    }
                }
                return Result.Succeeded;
            }

            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}
