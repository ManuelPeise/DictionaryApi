import { ITokenResponse } from './ITokenResponse';
import { IUserData } from './IUserData';

export interface IAuthenticationState {
  isAuthenticated: boolean;
  userData: IUserData | null;
  tokenData: ITokenResponse;
}
