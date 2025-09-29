export interface User {
  id: string;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  role?: string;
  isActive?: boolean;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface UserProfile {
  id: string;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  avatar?: string;
  role?: string;
}
