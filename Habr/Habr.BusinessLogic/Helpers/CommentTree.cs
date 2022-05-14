using AutoMapper;
using Habr.Common.DTO;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Helpers;

public static class CommentTree
{
    public static ICollection<Comment> SortToTree(IEnumerable<Comment> comments, int? commentId = null)
    {
        var result = new List<Comment>();
        var nextStepComments = new List<Comment>();
        foreach (var comment in comments)
        {
            if (comment.ParentCommentId == commentId)
            {
                result.Add(comment);
            }
            else
            {
                nextStepComments.Add(comment);
            }
        }

        foreach (var comment in result)
        {
            comment.Comments = SortToTree(nextStepComments, comment.Id);
        }

        return result;
    }
}