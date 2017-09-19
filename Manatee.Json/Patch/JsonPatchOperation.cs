using System.ComponentModel.DataAnnotations;

namespace Manatee.Json.Patch
{
    public enum JsonPatchOperation
    {
        [Display(Name = "add")]
        Add,
        [Display(Name = "remove")]
        Remove,
        [Display(Name = "replace")]
        Replace,
        [Display(Name = "move")]
        Move,
        [Display(Name = "copy")]
        Copy,
        [Display(Name = "test")]
        Test
    }
}