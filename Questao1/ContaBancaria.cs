using System;
using System.Globalization;

namespace Questao1
{
    public class ContaBancaria
    {
        public int numero;
        public string titular;
        private double depositoInicial;
        public double saldo;
        private const double taxaInstituicao = 3.50;

        public ContaBancaria(int numero, string titular, double depositoInicial = 0)
        {
            this.numero = numero;
            this.titular = titular;
            this.depositoInicial = depositoInicial;
            this.saldo = depositoInicial;
        }

        internal void Deposito(double quantia)
        {
            this.saldo += quantia;
        }

        internal void Saque(double quantia)
        {
            this.saldo -= quantia + taxaInstituicao;
        }
        public void AlterarNome(string nome)
        {
            titular = nome;
        }
        public override string ToString()
        {
            return String.Format("Conta {0}, Titular: {1}, Saldo: $ {2}", numero, titular, saldo);
        }

    }
}
