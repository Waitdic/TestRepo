import { createModel } from '@rematch/core';
import { isEmpty } from 'lodash';
//
import { RootModel } from '.';
import { getAwsJwtToken } from '@/utils/getAwsJwtToken';
import type { AppState, Module, Account, User } from '@/types';

export const app = createModel<RootModel>()({
  state: {
    modules: [],
    accounts: [],
    user: null,
    awsAmplify: { username: null, jwtToken: null },
    lang: window?.localStorage.getItem('lang') || navigator.language,
    theme:
      window?.localStorage.getItem('theme') ||
      window?.matchMedia('(prefers-color-scheme: dark)').matches
        ? 'dark'
        : 'light',
    error: null,
    isLoading: false,
    incompleteSetup: false,
    notification: null,
  } as AppState,
  reducers: {
    updateLang(state, payload: string) {
      window.localStorage.setItem('lang', payload);
      return {
        ...state,
        lang: payload,
      };
    },
    updateThemeColor(state, payload: string) {
      return { ...state, theme: payload };
    },
    updateUser(state, payload: User) {
      if (!isEmpty(payload)) {
        return { ...state, user: payload, accounts: [] };
      } else {
        return state;
      }
    },
    updateAwsUser(state, payload: { user: string; jwtToken: string }) {
      return {
        ...state,
        awsAmplify: { username: payload.user, jwtToken: payload.jwtToken },
      };
    },
    updateModuleList(state, payload: Module[]) {
      return {
        ...state,
        modules: payload,
      };
    },
    updateAccounts(state, payload: Account[]) {
      return {
        ...state,
        accounts: payload,
      };
    },
    setIsLoading(state, payload: boolean) {
      return {
        ...state,
        isLoading: payload,
      };
    },
    setError(state, payload: null | string | Error) {
      return { ...state, error: payload };
    },
    setIncompleteSetup(state, payload: boolean) {
      return { ...state, incompleteSetup: payload };
    },
    setNotification(state, payload: AppState['notification']) {
      return { ...state, notification: payload };
    },
    resetNotification(state) {
      return { ...state, notification: null };
    },
  },
  effects: (dispatch) => ({
    async getUserByAwsJwtToken(payload: { user: string | null }) {
      if (payload.user == null) return;
      try {
        const jwtToken = await getAwsJwtToken();
        dispatch.app.updateAwsUser({
          user: payload.user,
          jwtToken: jwtToken as string,
        });
      } catch (error) {
        if (typeof error === 'string') {
          dispatch.app.setError(error.toUpperCase());
        } else if (error instanceof Error) {
          dispatch.app.setError(error.message);
        }
      }
    },
    setThemeColor(payload: string) {
      const root = window?.document?.documentElement;

      if (root) {
        root?.classList.add(payload);
        window.localStorage.setItem('theme', payload);
        dispatch.app.updateThemeColor(payload);
      }
    },
    resetModuleList(_payload: string, state) {
      const fetchedModules = state.app.modules.map((module) => {
        if (module.uri === '/') {
          return { ...module, isActive: true };
        } else if (module.isActive) {
          return { ...module, isActive: false };
        } else {
          return module;
        }
      });
      dispatch.app.updateModuleList(fetchedModules);
    },
  }),
});
