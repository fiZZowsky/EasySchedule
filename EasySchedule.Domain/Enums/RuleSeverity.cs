namespace EasySchedule.Domain.Enums;

public enum RuleSeverity
{
    Hard,   // Zasada absolutna - nie może być złamana
    High,   // Ważna zasada - relaksowana w ostateczności
    Medium, // Średnia zasada  - relaksowana w drugiej kolejności
    Low     // Lekka zasada - relaksowana jako pierwsza
}