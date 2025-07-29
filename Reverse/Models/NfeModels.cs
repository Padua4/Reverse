using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reverse
{
    [XmlRoot("nfeProc", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class NfeProc
    {
        [XmlElement("NFe")]
        public NFe NFe { get; set; }
    }

    public class NFe
    {
        [XmlElement("infNFe")]
        public InfNFe InfNFe { get; set; }
    }

    public class InfNFe
    {
        [XmlElement("ide")]
        public Ide Ide { get; set; }

        [XmlElement("emit")]
        public Emit Emit { get; set; }

        [XmlElement("dest")]
        public Dest Dest { get; set; }

        [XmlElement("det")]
        public List<Det> Detalhes { get; set; }

        [XmlElement("total")]
        public Total Total { get; set; }
    }

    public class Ide
    {
        [XmlElement("nNF")]
        public string Numero { get; set; }

        [XmlElement("dhEmi")]
        public string DataHoraEmissao { get; set; }
    }

    public class Emit
    {
        [XmlElement("CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement("xNome")]
        public string Nome { get; set; }
    }

    public class Dest
    {
        [XmlElement("CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement("xNome")]
        public string Nome { get; set; }
    }

    public class Det
    {
        [XmlAttribute("nItem")]
        public int Item { get; set; }

        [XmlElement("prod")]
        public Produto Produto { get; set; }
    }

    public class Produto
    {
        [Key]
        [Column("cEAN")]
        [MaxLength(50)]
        public string CodigoBarras { get; set; }

        [Column("cProd")]
        public string Codigo { get; set; }

        public string Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal Valor { get; set; }
        public string CanalVenda { get; set; }
    }

    public class Total
    {
        [XmlElement("ICMSTot")]
        public ICMSTot ICMSTot { get; set; }
    }

    public class ICMSTot
    {
        [XmlElement("vNF")]
        public decimal ValorTotal { get; set; }
    }
}
