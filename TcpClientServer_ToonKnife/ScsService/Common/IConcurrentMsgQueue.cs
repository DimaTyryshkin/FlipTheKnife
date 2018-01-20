using Hik.Communication.Scs.Communication.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScsService.Common
{
    /// <summary>
    /// Сообщения асинхронно приходящие от мессенджеров(клиенты или серверы) класс складывает в очередь
    /// и позволяет их потом получить из главного игрового цикла
    /// </summary>
    public interface IConcurrentMsgQueue
    {
        /// <summary>
        /// Начать прием сообщений от клиента или сервера
        /// </summary>
        /// <param name="messenger">клиент или сервер</param>
        void AddMessenger(IMessenger messenger);


        void RemoveMessenger(IMessenger messenger);


        ReceivedMsg[] ToArray();
    }
}