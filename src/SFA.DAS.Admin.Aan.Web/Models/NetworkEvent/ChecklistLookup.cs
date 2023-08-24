namespace SFA.DAS.Admin.Aan.Web.Models.NetworkEvent;

//MFCMFC not sure if needed
public class ChecklistLookup
{
    public string Name { get; }
    public string Value { get; }
    public string Checked { get; set; } = string.Empty;

    public ChecklistLookup(string name, string value, bool isChecked = false)
    {
        Name = name;
        Value = value;
        Checked = isChecked ? "checked" : string.Empty;
    }
}
