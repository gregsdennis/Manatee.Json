/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ExpressionTreeBranch.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		ExpressionTreeBranch
	Purpose:		Represents a branch in an expression tree.

***************************************************************************************/
namespace Manatee.Json.Path.Expressions
{
	internal abstract class ExpressionTreeBranch<T> : ExpressionTreeNode<T>
	{
		public ExpressionTreeNode<T> Left { get; set; }
		public ExpressionTreeNode<T> Right { get; set; }

		protected bool Equals(ExpressionTreeBranch<T> other)
		{
			return Equals(Left, other.Left) && Equals(Right, other.Right);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ExpressionTreeBranch<T>);
		}
		public override int GetHashCode()
		{
			unchecked { return ((Left?.GetHashCode() ?? 0)*397) ^ (Right?.GetHashCode() ?? 0); }
		}
	}
}