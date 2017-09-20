using System.ComponentModel.DataAnnotations;

namespace Manatee.Json.Patch
{
    public enum JsonPatchOperation
    {
        [Display(Description = "add")]
        Add,
        [Display(Description = "remove")]
        Remove,
        [Display(Description = "replace")]
        Replace,
        [Display(Description = "move")]
        Move,
        [Display(Description = "copy")]
        Copy,
        [Display(Description = "test")]
        Test
    }
}