//using Intent.Modules.Common.CSharp.Templates;
//using Intent.Modules.Common.CSharp.TypeResolvers;
//using Intent.Modules.Common.TypeResolution;

//namespace Intent.Modules.Common.CSharp
//{
//    /// <summary>
//    /// Common CSharp collection formatters
//    /// </summary>
//    public static class CollectionFormatters
//    {
//        /// <summary>
//        /// Formats collections using <see cref="System.Collections.Generic.List{T}"/>
//        /// </summary>
//        public static ICollectionFormatter GetListFormatter()
//        {
//            return CSharpCollectionFormatter.GetOrCreate("System.Collections.Generic.List<{0}>");
//        }

//        /// <summary>
//        /// Formats collections using <see cref="System.Collections.Generic.IList{T}"/>
//        /// </summary>
//        public static ICollectionFormatter GetIListFormatter()
//        {
//            return CSharpCollectionFormatter.GetOrCreate("System.Collections.Generic.IList<{0}>");
//        }

//        /// <summary>
//        /// Formats collections using <see cref="System.Collections.Generic.ICollection{T}"/>
//        /// </summary>
//        public static ICollectionFormatter GetICollectionFormatter()
//        {
//            return CSharpCollectionFormatter.GetOrCreate("System.Collections.Generic.ICollection<{0}>");
//        }
//    }
//}