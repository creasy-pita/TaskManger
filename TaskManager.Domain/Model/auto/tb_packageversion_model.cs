using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TaskManager.Domain.Model
{
    /// <summary>
    /// tb_version Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_packageversion_model
    {
	/*�����Զ����ɹ����Զ�����,��Ҫ������д�Լ��Ĵ��룬����ᱻ�Զ�����Ŷ - ����*/
        
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int packageid { get; set; }
        
        /// <summary>
        /// �汾�� 
        /// </summary>
        public string versionno { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime createtime { get; set; }
        
        /// <summary>
        /// ѹ���ļ��������ļ�
        /// </summary>
        public byte[] zipfile { get; set; }
        
        /// <summary>
        /// zip�ļ���
        /// </summary>
        public string zipfilename { get; set; }
        
    }
}