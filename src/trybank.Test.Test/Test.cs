using Xunit;
using System.IO;
using FluentAssertions;
using trybank;
using System;

namespace trybank.Test;

public class TestReq1
{
    [Trait("Category", "1. Construa a funcionalidade de cadastrar novas contas")]
    [Theory(DisplayName = "Deve cadastrar contas com sucesso!")]
    [InlineData(2, 1, 123, 0)]
    public void TestRegisterAccountSucess(int number, int agency, int pass, int value)
    {        
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        Assert.Equal(1, trybank.registeredAccounts);
        Assert.Equal(trybank.Bank[0,0], number);
        Assert.Equal(trybank.Bank[0,1], agency);
        Assert.Equal(trybank.Bank[0,2], pass);
        Assert.Equal(trybank.Bank[0,3], value);
    }

    [Trait("Category", "1. Construa a funcionalidade de cadastrar novas contas")]
    [Theory(DisplayName = "Deve retornar ArgumentException ao cadastrar contas que já existem")]
    [InlineData(2, 1, 123)]
    public void TestRegisterAccountException(int number, int agency, int pass)
    {        
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        Action act = () => trybank.RegisterAccount(number, agency, pass);
        act.Should().Throw<ArgumentException>().WithMessage("A conta já está sendo usada!");

    }
}

public class TestReq2
{
    [Trait("Category", "2. Construa a funcionalidade de fazer Login")]
    [Theory(DisplayName = "Deve fazer login com sucesso!")]
    [InlineData(2, 1, 123, true)]
    public void TestLoginSucess(int number, int agency, int pass, bool logged)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        trybank.Login(number, agency, pass);
        Assert.Equal(trybank.Logged, logged);
    }

    [Trait("Category", "2. Construa a funcionalidade de fazer Login")]
    [Theory(DisplayName = "Deve informar que o usuário já está logado")]
    [InlineData(2, 1, 123)]
    public void TestLoginAccessViolation(int number, int agency, int pass)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        trybank.Login(number, agency, pass);
        Action act = () => trybank.Login(number, agency, pass);
        act.Should().Throw<AccessViolationException>().WithMessage("Usuário já está logado");
    }


    [Trait("Category", "2. Construa a funcionalidade de fazer Login")]
    [Theory(DisplayName = "Deve informar que a senha está incorreta")]
    [InlineData(2, 1, 123, 124)]
    public void TestLoginIncorrectPassword(int number, int agency, int pass, int wrongPass)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        Action act = () => trybank.Login(number, agency, wrongPass);
        act.Should().Throw<ArgumentException>().WithMessage("Senha incorreta");
    }


    [Trait("Category", "2. Construa a funcionalidade de fazer Login")]
    [Theory(DisplayName = "Deve informar que a agência e conta não foram encontradas")]
    [InlineData(2, 1, 123, 99)]
    public void TestLoginBankNotFounded(int number, int agency, int pass, int wrongAgency)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        Action act = () => trybank.Login(number, wrongAgency, pass);
        act.Should().Throw<ArgumentException>().WithMessage("Agência + Conta não encontrada");
    }

}

public class TestReq3
{
    [Trait("Category", "3. Construa a funcionalidade de fazer Logout")]
    [Theory(DisplayName = "Deve fazer logout com sucesso!")]
    [InlineData(2, 1, 123, false)]
    public void TestLogoutSucess(int number, int agency, int pass, bool logged)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        trybank.Login(number, agency, pass);
        trybank.Logout();
        Assert.Equal(trybank.Logged, logged);
    }

    [Trait("Category", "3. Construa a funcionalidade de fazer Logout")]
    [Theory(DisplayName = "Deve lançar erro caso não tenha feito login")]
    [InlineData("Usuário não está logado")]
    public void TestLogoutError(string message)
    {
        var trybank = new Trybank();
        Action act = () => trybank.Logout();
        act.Should().Throw<AccessViolationException>().WithMessage(message);
    }
}

public class TestReq4
{
    [Trait("Category", "4. Construa a funcionalidade de checar o saldo")]
    [Theory(DisplayName = "Deve checar o saldo com sucesso")]
    [InlineData(2, 1, 123, 0)]
    public void TestCheckBalance(int number, int agency, int pass, int value)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        trybank.Login(number, agency, pass);
        int balance = trybank.CheckBalance();
        Assert.Equal(balance, value);
    }

    [Trait("Category", "4. Construa a funcionalidade de checar o saldo")]
    [Theory(DisplayName = "Deve lançar erro caso não tenha feito login")]
    [InlineData("Usuário não está logado")]
    public void TestCheckBalanceError(string message)
    {
        var trybank = new Trybank();
        Action act = () => trybank.CheckBalance();
        act.Should().Throw<AccessViolationException>().WithMessage(message);
    }
}

public class TestReq5
{
    [Trait("Category", "5. Construa a funcionalidade de depositar dinheiro")]
    [Theory(DisplayName = "Deve depositar dinheiro com sucesso")]
    [InlineData(2, 1, 123, 1000)]
    public void TestDeposit(int number, int agency, int pass, int value)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        trybank.Login(number, agency, pass);
        trybank.Deposit(value);
        Assert.Equal(trybank.Bank[0,3], value);
    }

    [Trait("Category", "5. Construa a funcionalidade de depositar dinheiro")]
    [Theory(DisplayName = "Deve lançar erro caso não tenha feito login")]
    [InlineData("Usuário não está logado", 1000)]
    public void TestDepositError(string message, int value)
    {
        var trybank = new Trybank();
        Action act = () => trybank.Deposit(value);
        act.Should().Throw<AccessViolationException>().WithMessage(message);
    }
}

public class TestReq6
{
    [Trait("Category", "6. Construa a funcionalidade de sacar dinheiro")]
    [Theory(DisplayName = "Deve sacar dinheiro com sucesso")]
    [InlineData(2, 1, 123, 1050)]
    public void TestWithdraw(int number, int agency, int pass, int value)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        trybank.Bank[0,3] = 1500;
        trybank.Login(number, agency, pass);
        trybank.Withdraw(450);
        Assert.Equal(trybank.Bank[0,3], value);
    }

    [Trait("Category", "6. Construa a funcionalidade de sacar dinheiro")]
    [Theory(DisplayName = "Deve lançar erro caso não tenha feito login")]
    [InlineData("Usuário não está logado", 1000)]
    public void TestWithdrawError(string message, int value)
    {
        var trybank = new Trybank();
        Action act = () => trybank.Withdraw(value);
        act.Should().Throw<AccessViolationException>().WithMessage(message);
    }

    [Trait("Category", "6. Construa a funcionalidade de sacar dinheiro")]
    [Theory(DisplayName = "Deve lançar erro caso o saldo seja insuficiente")]
    [InlineData("Saldo insuficiente", 2, 1, 123, 2000)]
    public void TestInvalidOperationError(string message, int number, int agency, int pass, int value)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(number, agency, pass);
        trybank.Bank[0,3] = 1500;
        trybank.Login(number, agency, pass);
        Action act = () => trybank.Withdraw(value);
        act.Should().Throw<InvalidOperationException>().WithMessage(message);
    }
}

public class TestReq7
{
    [Trait("Category", "7. Construa a funcionalidade de transferir dinheiro entre contas")]
    [Theory(DisplayName = "Deve sacar dinheiro com sucesso")]
    [InlineData(2, 1, 123, 5, 4, 456, 1550, 2450)]
    public void TestTransfer(int originNumber, int originAgency, int originPass, int destinyNumber, int destinyAgency, int destinyPass, int originValue, int destinyValue)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(originNumber, originAgency, originPass);
        trybank.Login(originNumber, originAgency, originPass);
        trybank.RegisterAccount(destinyNumber, destinyAgency, destinyPass);
        trybank.Bank[0,3] = 2000;
        trybank.Bank[1,3] = 2000;

        trybank.Transfer(destinyNumber, destinyAgency, 450);
        Assert.Equal(trybank.Bank[0,3], originValue);
        Assert.Equal(trybank.Bank[1,3], destinyValue);
    }

    [Trait("Category", "7. Construa a funcionalidade de transferir dinheiro entre contas")]
    [Theory(DisplayName = "Deve lançar erro caso não tenha feito login")]
    [InlineData("Usuário não está logado", 1000)]
    public void TestTransferError(string message, int value)
    {
        var trybank = new Trybank();
        Action act = () => trybank.Transfer(0, 0, value);
        act.Should().Throw<AccessViolationException>().WithMessage(message);
    }

    [Trait("Category", "7. Construa a funcionalidade de transferir dinheiro entre contas")]
    [Theory(DisplayName = "Deve lançar erro caso o saldo seja insuficiente")]
    [InlineData(2, 1, 123, 5, 4, 456, "Saldo insuficiente")]
    public void InvalidOperationError(int originNumber, int originAgency, int originPass, int destinyNumber, int destinyAgency, int destinyPass, string message)
    {
        var trybank = new Trybank();
        trybank.RegisterAccount(originNumber, originAgency, originPass);
        trybank.Login(originNumber, originAgency, originPass);
        trybank.RegisterAccount(destinyNumber, destinyAgency, destinyPass);
        trybank.Bank[0,3] = 300;
        trybank.Bank[1,3] = 2000;

        Action act = () => trybank.Transfer(destinyNumber, destinyAgency, 450);
        act.Should().Throw<InvalidOperationException>().WithMessage(message);
    }

}