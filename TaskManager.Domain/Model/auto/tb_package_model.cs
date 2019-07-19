using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TaskManager.Domain.Model
{
    /// <summary>
    /// tb_version Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_package_model
    {
	/*�����Զ����ɹ����Զ�����,��Ҫ������д�Լ��Ĵ��룬����ᱻ�Զ�����Ŷ - ����*/
        
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        
        
        /// <summary>
        /// ���������
        /// </summary>
        public string packagename { get; set; }


        /// <summary>
        /// ����
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime createtime { get; set; }
        
        
    }
}