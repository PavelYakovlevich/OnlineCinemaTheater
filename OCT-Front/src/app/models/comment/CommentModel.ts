export interface CommentModel {
    id: string;
    mediaId: string;
    userId: string;
    text: string;
    issueDate: string;
    user: UserCommentModel;
}

export interface UserCommentModel {
    name: string;
    surname: string;
}