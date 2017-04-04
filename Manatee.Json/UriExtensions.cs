﻿using System;

namespace Manatee.Json
{
	// Source: https://github.com/WebDAVSharp/WebDAVSharp.Server/blob/1d2086a502937936ebc6bfe19cfa15d855be1c31/WebDAVExtensions.cs
	internal static class UriExtensions
	{
		/// <summary>
		/// Gets the Uri to the parent object.
		/// </summary>
		/// <param name="uri">The <see cref="Uri" /> of a resource, for which the parent Uri should be retrieved.</param>
		/// <returns>
		/// The parent <see cref="Uri" />.
		/// </returns>
		/// <exception cref="ArgumentNullException">uri</exception>
		/// <exception cref="InvalidOperationException">Cannot get parent of root</exception>
		/// <exception cref="ArgumentNullException"><paramref name="uri" /> is <c>null</c>.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="uri" /> has no parent, it refers to a root resource.</exception>
		public static Uri GetParentUri(this Uri uri)
		{
			if (uri == null) throw new ArgumentNullException(nameof(uri));
			if (uri.Segments.Length == 1) throw new InvalidOperationException("Cannot get parent of root");

			string url = uri.ToString();
			int index = url.Length - 1;
			if (url[index] == '/')
				index--;
			while (url[index] != '/')
				index--;
			return new Uri(url.Substring(0, index + 1));
		}
	}
}
