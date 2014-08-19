/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ExpressionTreeNode.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		ExpressionTreeNode
	Purpose:		Represents a single node in an expression tree.

***************************************************************************************/
namespace Manatee.Json.Path.Expressions
{
	internal abstract class ExpressionTreeNode<T>
	{
		public abstract int Priority { get; }

		public abstract object Evaluate(T json);
	}
}