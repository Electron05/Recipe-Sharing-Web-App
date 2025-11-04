import { RecipeDtoFeed } from './get-recipe';

export interface UserProfileDtoDisplay {
  id: number;
  isDeleted: boolean;
  username: string;
  bio?: string | null;
  profilePictureUrl?: string | null;
  createdAt: string;
  recipes: RecipeDtoFeed[];
  followersCount: number;
  followingCount: number;
  isFollowing: boolean;
}
