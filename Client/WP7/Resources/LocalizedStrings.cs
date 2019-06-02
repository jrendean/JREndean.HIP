//
// Copyright 2011 - JR Endean
//

namespace JREndean.HIP.Client.WP7
{
    using System;
    using JREndean.HIP.Client.WP7.Resources;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
    public class LocalizedStrings
    {
        private static readonly Strings resource = new Strings();

        public LocalizedStrings()
        {
        }

        public static Strings Resource
        {
            get { return resource; }
        }
    }
}
