using notissimus.toyparser.core.Enums;

namespace notissimus.toyparser.core;

public static class Region
{
    public static string EnumToCookie(CityEnum city) => city switch
    {
        CityEnum.Moscow => "BITRIX_SM_city=77000000000",
        CityEnum.SaintPetersburg => "BITRIX_SM_city=78000000000",
        CityEnum.RostovOnDon => "BITRIX_SM_city=61000001000",
        _ => String.Empty
    };
}