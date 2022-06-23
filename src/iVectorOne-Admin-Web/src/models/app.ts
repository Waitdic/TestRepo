import { createModel } from '@rematch/core';
//
import { RootModel } from '.';
import {
  AppState,
  Module,
  Provider,
  Subscription,
  Tenant,
  User,
} from '@/types';
import { isEmpty } from 'lodash';
import { getAwsJwtToken } from '@/utils/getAwsJwtToken';

export const app = createModel<RootModel>()({
  state: {
    modules: [],
    subscriptions: [],
    providers: [],
    user: null,
    awsAmplify: { username: null, jwtToken: null },
    lang: window?.localStorage.getItem('lang') || navigator.language,
    theme:
      window?.localStorage.getItem('theme') ||
      window?.matchMedia('(prefers-color-scheme: dark)').matches
        ? 'dark'
        : 'light',
    error: null,
    signOut: () => void 0,
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
        return { ...state, user: payload };
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
    updateTenantList(state, payload: Tenant[]) {
      return {
        ...state,
        tenants: payload,
      };
    },
    updateSubscriptions(state, payload: Subscription[]) {
      return {
        ...state,
        subscriptions: payload,
      };
    },
    updateProviders(state, payload: Provider[]) {
      return {
        ...state,
        providers: payload,
      };
    },
    setError(state, payload: null | string | Error) {
      return { ...state, error: payload };
    },
    assignSignOut(state, payload: () => void) {
      return { ...state, signOut: payload };
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
    setSignOutCallback(payload: () => void) {
      dispatch.app.assignSignOut(payload);
    },
    setThemeColor(payload: string) {
      const root = window?.document?.documentElement;

      if (root) {
        root?.classList.add(payload);
        window.localStorage.setItem('theme', payload);
        dispatch.app.updateThemeColor(payload);
      }
    },
    resetModuleList(payload: string, state) {
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
