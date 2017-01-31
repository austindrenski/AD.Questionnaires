//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Xml.Linq;
//using JetBrains.Annotations;

//namespace QuestionnaireExtractionLibrary
//{
    //[PublicAPI]
    //internal static class XmlMinification
    //{
    //    [SuppressMessage("ReSharper", "TailRecursiveCall")]
    //    private static XElement MinifyRunNodes(this XElement element)
    //    {
    //        if (element.HasElements)
    //        {
    //            return new XElement(element.Name.LocalName, element.Elements().Select(x => x.MinifyRunNodes()));
    //        }
    //        if (element.Name != "r")
    //        {
    //            return element;
    //        }
    //        XElement next = (XElement)element.NextNode;
    //        if (next?.Name != "r")
    //        {
    //            return element;
    //        }
    //        element.Value += next.Value;
    //        next.Remove();
    //        return element.MinifyRunNodes();
    //    }

    //    private static XElement MinifyTextNodes(this XElement element)
    //    {
    //        if (!element.HasElements)
    //        {
    //            return element;
    //        }
    //        return element.Name == "r" ? new XElement(element.Name.LocalName, element.Value)
    //                                   : new XElement(element.Name.LocalName, element.Elements().Select(x => x.MinifyTextNodes()));
    //    }
    //}
//}
