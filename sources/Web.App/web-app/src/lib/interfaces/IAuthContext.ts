import { IAuthenticationRequest } from './IAuthenticationRequest';
import { ITokenResponse } from './ITokenResponse';
import { IUserData } from './IUserData';

export interface IAuthContext {
  isAuthenticated: boolean;
  userData: IUserData | null;
  getTokens: () => ITokenResponse | null;
  onLogin: (authData: IAuthenticationRequest) => Promise<void>;
  onLogout: () => void;
}
