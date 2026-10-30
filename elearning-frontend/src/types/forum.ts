// Remove LocalUser import and update ForumPost interface

export interface ForumPost {
  id: number;
  title: string;
  content: string;
  createdAt: string;
  updatedAt?: string;
  isPinned: boolean;
  author: {
    id: string;
    firstName: string;
    lastName: string;
    fullName: string;
    profileImageUrl?: string;
    email:string;
    role: string;
    createdAt: string;
    // Add any other necessary fields that match your actual data
  };
  courseId: number;
  parentPostId?: number;
  replies: ForumPost[];
}

export interface CreateForumPost {
  title: string;
  content: string;
  courseId: number;
  parentPostId?: number;
}