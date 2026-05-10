using Stateless;

public enum State
{
    NewDefect, 
    Triage,    
    Fixing,   
    Return,   
    Closed,    
    Reopened   
}

public enum Trigger
{
    StartTriage,     
    NeedMoreInfo,     
    NoTime,        
    SeparateSolution, 
    OtherProduct,     
    AssignFix,       
    NotABug,         
    WontFix,        
    Duplicate,       
    CannotRepro,      
    ReturnToTriage,   
    FixOk,            
    FixNotOk,       
    ProblemSolved,    
    ProblemNotSolved,
    Reopen            
}

public class Bug
{
    private readonly StateMachine<State, Trigger> _machine;

    public State CurrentState => _machine.State;

    public Bug(State initialState = State.NewDefect)
    {
        _machine = new StateMachine<State, Trigger>(initialState);

        _machine.Configure(State.NewDefect)
            .Permit(Trigger.StartTriage, State.Triage);

        _machine.Configure(State.Triage)
            .PermitReentry(Trigger.NeedMoreInfo)
            .PermitReentry(Trigger.NoTime)
            .PermitReentry(Trigger.SeparateSolution)
            .PermitReentry(Trigger.OtherProduct)
            .Permit(Trigger.AssignFix, State.Fixing)
            .Permit(Trigger.NotABug, State.Return)
            .Permit(Trigger.WontFix, State.Return)
            .Permit(Trigger.Duplicate, State.Return)
            .Permit(Trigger.CannotRepro, State.Return)
            .Permit(Trigger.ProblemSolved, State.Closed)
            .Permit(Trigger.ProblemNotSolved, State.Reopened);

    
        _machine.Configure(State.Fixing)
            .Permit(Trigger.FixOk, State.Triage)   
            .Permit(Trigger.FixNotOk, State.Return); 

        _machine.Configure(State.Return)
            .Permit(Trigger.ReturnToTriage, State.Triage);

        _machine.Configure(State.Reopened)
            .Permit(Trigger.Reopen, State.Triage);

        _machine.Configure(State.Closed)
            .Permit(Trigger.Reopen, State.Triage);
    }

    public void StartTriage()      => _machine.Fire(Trigger.StartTriage);
    public void NeedMoreInfo()     => _machine.Fire(Trigger.NeedMoreInfo);
    public void NoTime()           => _machine.Fire(Trigger.NoTime);
    public void SeparateSolution() => _machine.Fire(Trigger.SeparateSolution);
    public void OtherProduct()     => _machine.Fire(Trigger.OtherProduct);
    public void AssignFix()        => _machine.Fire(Trigger.AssignFix);
    public void NotABug()          => _machine.Fire(Trigger.NotABug);
    public void WontFix()          => _machine.Fire(Trigger.WontFix);
    public void Duplicate()        => _machine.Fire(Trigger.Duplicate);
    public void CannotRepro()      => _machine.Fire(Trigger.CannotRepro);
    public void ReturnToTriage()   => _machine.Fire(Trigger.ReturnToTriage);
    public void FixOk()            => _machine.Fire(Trigger.FixOk);
    public void FixNotOk()         => _machine.Fire(Trigger.FixNotOk);
    public void ProblemSolved()    => _machine.Fire(Trigger.ProblemSolved);
    public void ProblemNotSolved() => _machine.Fire(Trigger.ProblemNotSolved);
    public void Reopen()           => _machine.Fire(Trigger.Reopen);

    public bool CanFire(Trigger trigger) => _machine.CanFire(trigger);
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Bug Workflow Demo ===\n");

        var bug1 = new Bug();
        Console.WriteLine($"[Bug1] Новый дефект: {bug1.CurrentState}");
        bug1.StartTriage();
        Console.WriteLine($"[Bug1] Разбор: {bug1.CurrentState}");
        bug1.AssignFix();
        Console.WriteLine($"[Bug1] Исправление: {bug1.CurrentState}");
        bug1.FixOk();
        Console.WriteLine($"[Bug1] OK=ДА → разбор: {bug1.CurrentState}");
        bug1.ProblemSolved();
        Console.WriteLine($"[Bug1] Проблема решена → Закрыт: {bug1.CurrentState}");

        Console.WriteLine();

        var bug2 = new Bug();
        bug2.StartTriage();
        bug2.NeedMoreInfo();
        Console.WriteLine($"[Bug2] Нужно больше инфо (остался в разборе): {bug2.CurrentState}");
        bug2.AssignFix();
        bug2.FixOk();
        bug2.ProblemSolved();
        Console.WriteLine($"[Bug2] Закрыт: {bug2.CurrentState}");

        Console.WriteLine();

        var bug3 = new Bug();
        bug3.StartTriage();
        bug3.NotABug();
        Console.WriteLine($"[Bug3] Не дефект → Возврат: {bug3.CurrentState}");
        bug3.ReturnToTriage();
        Console.WriteLine($"[Bug3] Обратно в разбор: {bug3.CurrentState}");
        bug3.AssignFix();
        bug3.FixOk();
        bug3.ProblemSolved();
        Console.WriteLine($"[Bug3] Закрыт: {bug3.CurrentState}");

        Console.WriteLine();

        var bug4 = new Bug();
        bug4.StartTriage();
        bug4.AssignFix();
        bug4.FixNotOk();
        Console.WriteLine($"[Bug4] OK=НЕТ → Возврат: {bug4.CurrentState}");
        bug4.ReturnToTriage();
        bug4.AssignFix();
        bug4.FixOk();
        bug4.ProblemNotSolved();
        Console.WriteLine($"[Bug4] Проблема не решена → Переоткрыт: {bug4.CurrentState}");
        bug4.Reopen();
        Console.WriteLine($"[Bug4] Переоткрыт → Разбор: {bug4.CurrentState}");

        Console.WriteLine("\n=== Демонстрация завершена ===");
    }
}
