using Table;
using Xaz;

public class UserBagData : UserBagDataBase
{
    public UserBagData() : base()
    {
        
       
    }

    public UserCategoryData GetCategoryInfo()
    {
        return new UserCategoryData(itemType, itemId, num);
    }



}
