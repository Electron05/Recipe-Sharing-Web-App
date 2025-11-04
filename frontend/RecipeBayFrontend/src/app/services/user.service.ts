import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RecipeDtoDisplay } from '../types/get-recipe';
import { UserProfileDtoDisplay } from '../types/user';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private http: HttpClient) {}

  getProfile(id: number): Observable<UserProfileDtoDisplay> {
    return this.http.get<UserProfileDtoDisplay>(`${environment.apiUrl}/Users/profile/${id}`);
  }

  setBio(bio: string): Observable<UserProfileDtoDisplay> {
    return this.http.post<UserProfileDtoDisplay>(`${environment.apiUrl}/Users/setbio`, { bio });
  }

  followUser(userId: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/Users/${userId}/follow`, {});
  }

  unfollowUser(userId: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/Users/${userId}/unfollow`, {});
  }
}
