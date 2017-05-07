using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//NotificationCenter用于处理游戏物体之间的消息。  
//游戏物体可以注册以接收特定的通知。当另一个对象发送该类型的通知，所有注册并落实相应的消息游戏物体将收到通知。  
//五合一游戏物体必须注册才能获得与addObserver的功能的通知，并通过他们的自我，以及通知的名称。观察游戏物体也可以注销自己与RemoveObserver功能。游戏物体必须要求按类型基础上接收和删除一个类型的通知类型。  
//发布通知是通过创建一个Notification对象并将它传递给PostNotification完成。所有接收游戏物体将接受通知的对象。通知对象包含发送者，通知类型名称，并包含数据的选项哈希表。  
//要使用NotificationCenter，无论是创建和管理地方它唯一的实例，或者使用静态NotificationCenter。  
   
//我们需要一个静态方法的对象能够获得默认的通知中心。  
//这个默认的中心是所有的对象都将使用什么最通知。我们当然可以创建自己的独立NotificationCenter的实例，但这是静态的由所有。  
public class NotificationCenter : MonoBehaviour
{
    private static NotificationCenter defaultCenter;
    //单例
    public static NotificationCenter DefaultCenter()
    {
        // 如果defaultCenter不存在，我们需要创建它
        if (!defaultCenter)
        {
            //因为NotificationCenter是一个组件，我们必须创建一个游戏物体将其附加到。  
            GameObject notificationObject = new GameObject("Default Notification Center");
            // 添加NotificationCenter组件，并将其设置为defaultCenter  
            defaultCenter = notificationObject.AddComponent<NotificationCenter>();
            DontDestroyOnLoad(defaultCenter);
        }

        return defaultCenter;
    }

    // notification的哈希表，每个notification是一个Arraylist，包含所有observer
    Hashtable notifications = new Hashtable();
    
    //为一个消息绑定一个接受者，name为消息名，observer为接受者
    public void AddObserver(Component observer, String name) { AddObserver(observer, name, null); }
    public void AddObserver(Component observer, String name, object sender)
    {
        if (name == null || name == "") { Debug.Log("Null name specified for notification in AddObserver."); return; }
        if (!notifications.ContainsKey(name))
        {
            notifications[name] = new List<Component>();
        }


        List<Component> notifyList = (List<Component>)notifications[name];
        
        if (!notifyList.Contains(observer)) { notifyList.Add(observer); }
    }

    //删除观察者 
    public void RemoveObserver(Component observer, String name)
    {
        List<Component> notifyList = (List<Component>)notifications[name]; //change from original

        // 假设这是一个有效的通知类型，从列表中删除观察者。 
        // 如果观察员名单现在是空的，然后从通知散列删除通知类型。这是看家的目的。  
        if (notifyList != null)
        {
            if (notifyList.Contains(observer)) { notifyList.Remove(observer); }
            if (notifyList.Count == 0) { notifications.Remove(name); }
        }
    }

   //发送消息 
    public void PostNotification(Component aSender, String aName) { PostNotification(aSender, aName, null); }
    public void PostNotification(Component aSender, String aName, object aData) { PostNotification(new Notification(aSender, aName, aData)); }
    public void PostNotification(Notification aNotification)
    {
        // 首先确保该通知的名称是有效的。  
        if (aNotification.name == null || aNotification.name == "") { Debug.Log("Null name sent to PostNotification."); return; }
        // /获取通知列表，并确保它是有效的
        List<Component> notifyList = (List<Component>)notifications[aNotification.name]; //change from original
        if (notifyList == null) { Debug.Log("Notify list not found in PostNotification."); return; }

        // 克隆列表中，所以不会有一个问题，如果一个观察者添加或删除，同时通知被发送 
        notifyList = new List<Component>(notifyList);

        // 创建一个数组来保存我们需要删除无效的观察员轨道  
        List<Component> observersToRemove = new List<Component>(); //change from original

        //Itterate通过所有已签署了该类型的通知被通知的对象。  
        foreach (Component observer in notifyList)
        {
            // 如果观察者是无效的，然后跟踪它，所以我们以后可以将其删除。  
            // 我们无法将其删除，现在，还是会乱for循环起来。  
            if (!observer)
            {
                observersToRemove.Add(observer);
            }
            else
            {
                // 如果观察者是有效的，然后发送该通知。这是发送的消息是通知的名称。  
                observer.SendMessage(aNotification.name, aNotification, SendMessageOptions.DontRequireReceiver);
            }
        }

        // 删除所有无效的观察者  
        foreach (Component observer in observersToRemove)
        {
            notifyList.Remove(observer);
        }
    }
}

// 通知类是发送到接收通知类型的对象的对象。  
// 这个类包含发送游戏物体，该通知的名称，以及任选的含有哈希表的数据。  
public class Notification
{
    public Component sender;
    public String name;
    public object data;

    public Notification(Component aSender, String aName) { sender = aSender; name = aName; data = null; }
    public Notification(Component aSender, String aName, object aData) { sender = aSender; name = aName; data = aData; }
}