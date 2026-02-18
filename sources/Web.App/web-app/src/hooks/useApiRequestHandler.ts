import React from 'react';
import { useAuth } from './useAuth';

export const useApiRequestHandler = () => {
  const { getTokens } = useAuth();

  const addAuthHeader = React.useCallback(
    (headers: HeadersInit): HeadersInit => {
      const tokens = getTokens();
      if (tokens) {
        headers = {
          ...headers,
          Authorization: `Bearer ${tokens.accessToken}`,
        };
      }
      return headers;
    },
    [getTokens],
  );

  const sendGetRequest = React.useCallback(
    async <TModel>(url: string, params?: Record<string, string>): Promise<TModel> => {
      const requestUrl = params ? `${url}?${new URLSearchParams(params).toString()}` : url;
      const response = await fetch(requestUrl, {
        method: 'GET',
        mode: 'cors',
        headers: addAuthHeader({ 'Content-Type': 'application/json' }),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorText}`,
        );
      }
      return response.json() as Promise<TModel>;
    },
    [addAuthHeader],
  );

  const sendPostRequest = React.useCallback(
    async <TModel>(url: string, body: any, params?: Record<string, string>): Promise<TModel> => {
      const requestUrl = params ? `${url}?${new URLSearchParams(params).toString()}` : url;
      const response = await fetch(requestUrl, {
        method: 'POST',
        mode: 'cors',
        headers: addAuthHeader({ 'Content-Type': 'application/json' }),
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorText}`,
        );
      }
      return response.json() as Promise<TModel>;
    },
    [addAuthHeader],
  );

  return {
    sendGetRequest,
    sendPostRequest,
  };
};
