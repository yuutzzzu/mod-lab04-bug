using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BugTests;

[TestClass]
public class WorkflowScenarios
{
    [TestMethod]
    public void CreateNewItem_DefaultStatusIsOpen()
    {
        var bug = new Bug();
        Assert.AreEqual(State.NewDefect, bug.CurrentState);
    }

    [TestMethod]
    public void StartReview_MovesToReviewStage()
    {
        var bug = new Bug();
        bug.StartTriage();
        Assert.AreEqual(State.Triage, bug.CurrentState);
    }

    [TestMethod]
    public void AcceptAndAssign_MovesToInProgress()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.AssignFix();
        Assert.AreEqual(State.Fixing, bug.CurrentState);
    }

    [TestMethod]
    public void MarkFixed_SendsBackForVerification()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.AssignFix();
        bug.FixOk();
        Assert.AreEqual(State.Triage, bug.CurrentState);
    }

    [TestMethod]
    public void ConfirmResolved_ClosesItem()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.AssignFix();
        bug.FixOk();
        bug.ProblemSolved();
        Assert.AreEqual(State.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void ReopenFromClosed_ReturnsToReview()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.AssignFix();
        bug.FixOk();
        bug.ProblemSolved();
        bug.Reopen();
        Assert.AreEqual(State.Triage, bug.CurrentState);
    }

    [TestMethod]
    public void MarkAsInvalid_MovesToRejected()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.NotABug();
        Assert.AreEqual(State.Return, bug.CurrentState);
    }

    [TestMethod]
    public void RejectAndReturn_CanBeRestarted()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.NotABug();
        bug.ReturnToTriage();
        Assert.AreEqual(State.Triage, bug.CurrentState);
    }

    [TestMethod]
    public void FixRejected_SendsBackToRejected()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.AssignFix();
        bug.FixNotOk();
        Assert.AreEqual(State.Return, bug.CurrentState);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void AssignWithoutReview_ThrowsError()
    {
        var bug = new Bug();
        bug.AssignFix();
    }

    [TestMethod]
    public void MarkAsDuplicate_GoesToRejected()
    {
        var bug = new Bug();
        bug.StartTriage();
        bug.Duplicate();
        Assert.AreEqual(State.Return, bug.CurrentState);
    }
}
