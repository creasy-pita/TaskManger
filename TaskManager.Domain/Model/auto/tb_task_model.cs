using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TaskManager.Domain.Model
{
    /// <summary>
    /// tb_task Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_task_model
    {
	/*�����Զ����ɹ����Զ�����,��Ҫ������д�Լ��Ĵ��룬����ᱻ�Զ�����Ŷ - ����*/
        
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string taskname { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int categoryid { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int nodeid { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime taskcreatetime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime taskupdatetime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime tasklaststarttime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime tasklastendtime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime tasklasterrortime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int taskerrorcount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int taskruncount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int taskcreateuserid { get; set; }
        
        /// <summary>
        /// 0 ֹͣ  1 ������
        /// </summary>
        public Byte taskstate { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int taskversion { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string taskappconfigjson { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string taskcron { get; set; }

        /// <summary>
        /// ������ʹ�� obsolete discard
        /// </summary>
        public string taskmainclassnamespace { get; set; }

        /// <summary>
        /// ������ʹ�� obsolete discard
        /// </summary>
        public string taskmainclassdllfilename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string taskremark { get; set; }

        /// <summary>
        /// �����Ŀ¼ rootpath
        /// </summary>
        public string taskpath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string taskstartfilename { get; set; }

        /// <summary>
        /// ��������������нű��е�  ����(���� java -jar xxx.jar ��  ��-jar xxx.jar)
        /// </summary>
        public string taskarguments { get; set; }

        /// <summary>
        /// ���Ҵ��������id��������ű���һ��������powershell����cmd Ŀǰ֧��powershell�Ľű�
        /// </summary>
        public string taskfindbatchscript { get; set; }

        /// <summary>
        /// ж�س����ǰ��Ҫִ�еĽű������� ���� ���window�����ע��Ľű�
        /// </summary>
        public string taskuninstallbatchscript { get; set; }

    }
}