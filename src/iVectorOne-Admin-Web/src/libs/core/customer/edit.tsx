import { FC, useEffect, useState, useCallback, Fragment } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { Helmet } from 'react-helmet-async';
import { useForm, SubmitHandler } from 'react-hook-form';
import { get } from 'lodash';
import { Dialog, Transition } from '@headlessui/react';
//
import { countries } from '@/temp';
import { RootState } from '@/store';
import { ButtonColors, ButtonVariants, InputTypes } from '@/constants';
import { translateMessage } from '@/utils/translate-message';
import MainLayout from '@/layouts/Main';
//
import {
  TextField,
  Select,
  TextArea,
  Checkbox,
  Radio,
  Modal,
  Button,
} from '@/components';

type UserFormData = {
  username: string;
  about: string;
  firstName: string;
  lastName: string;
  email: string;
  country: string;
  streetAddress: string;
  city: string;
  state: string;
  postCode: string;
  comments: boolean;
  candidates: boolean;
  offers: boolean;
  pushNotifications: string;
};

export const CustomerEdit: FC = () => {
  const user = useSelector((state: RootState) => state.app.user);
  const navigate = useNavigate();

  const [title, setTitle] = useState<string>('Intuitive Admin');
  const [deleteModelOpen, setDeleteModelOpen] = useState<boolean>(false);

  const {
    register,
    handleSubmit,
    setValue,
    getValues,
    formState: { errors },
  } = useForm<UserFormData>();

  const onSubmit: SubmitHandler<UserFormData> = (data) => {
    const resultData = {
      ...data,
      email: `${data.email}@example.com`,
    };
  };

  const setDefaultValues = useCallback(() => {
    const username = get(user, 'username', '');
    const about = get(user, 'about', '');
    const firstName = get(user, 'name.first', '');
    const lastName = get(user, 'name.last', '');
    const email = get(user, 'email', '').split('@');
    const streetAddress = get(user, 'location.street', '');
    const city = get(user, 'location.city', '');
    const state = get(user, 'location.state', '');
    const postCode = get(user, 'location.postcode', '');

    setValue('username', username);
    setValue('about', about);
    setValue('firstName', firstName);
    setValue('lastName', lastName);
    setValue('email', email[0]);
    setValue(
      'streetAddress',
      `${streetAddress?.number} ${streetAddress?.name}`
    );
    setValue('city', city);
    setValue('state', state);
    setValue('postCode', postCode);
  }, [setValue, user]);

  useEffect(() => {
    if (user) {
      const fullName = user.fullName;
      const name =
        fullName.length > 16 ? `${fullName.substring(0, 16)}...` : fullName;
      setTitle(name);
      setDefaultValues();
    }
  }, [user, setDefaultValues]);

  return (
    <>
      <Helmet>
        <title>{title}</title>
      </Helmet>

      <MainLayout
        title={translateMessage('app.customer.edit.with.name', user?.fullName)}
      >
        <div className='py-5'>
          <form
            className='space-y-8 divide-y divide-gray-200'
            onSubmit={handleSubmit(onSubmit)}
            autoComplete='turnedOff'
          >
            <div className='space-y-8 divide-y divide-gray-200'>
              <div>
                <div>
                  <h3 className='text-lg leading-6 font-medium text-gray-900'>
                    Profile
                  </h3>
                  <p className='mt-1 text-sm text-gray-500'>
                    This information will be displayed publicly so be careful
                    what you share.
                  </p>
                </div>
                <div className='mt-6 grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6'>
                  <div className='sm:col-span-4 max-w-max'>
                    <TextField
                      id='username'
                      {...register('username', {
                        required: 'This field is required.',
                      })}
                      labelText='Username'
                      isDirty={errors.username ? true : false}
                      errorMsg={errors.username?.message}
                    />
                  </div>
                  <div className='sm:col-span-6'>
                    <TextArea
                      id='about'
                      labelText='About'
                      {...register('about', {
                        required: 'This field is required.',
                      })}
                      defaultValue={getValues('about')}
                      isDirty={errors.about ? true : false}
                      errorMsg={errors.about?.message}
                    />
                    <p className='mt-2 text-sm text-gray-500'>
                      Write a few sentences about yourself.
                    </p>
                  </div>
                  {/* TODO: Create Photo change UI lib */}
                  <div className='sm:col-span-6'>
                    <label
                      htmlFor='photo'
                      className='block text-sm font-medium text-gray-700'
                    >
                      Photo
                    </label>
                    <div className='mt-1 flex items-center'>
                      <span className='h-12 w-12 rounded-full overflow-hidden bg-gray-100'>
                        <svg
                          className='h-full w-full text-gray-300'
                          fill='currentColor'
                          viewBox='0 0 24 24'
                        >
                          <path d='M24 20.993V24H0v-2.996A14.977 14.977 0 0112.004 15c4.904 0 9.26 2.354 11.996 5.993zM16.002 8.999a4 4 0 11-8 0 4 4 0 018 0z' />
                        </svg>
                      </span>
                      <button
                        type='button'
                        className='ml-5 bg-white py-2 px-3 border border-gray-300 rounded-md shadow-sm text-sm leading-4 font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500'
                      >
                        Change
                      </button>
                    </div>
                  </div>
                  {/* TODO: Create Cover photo UI lib */}
                  <div className='sm:col-span-6'>
                    <label
                      htmlFor='cover-photo'
                      className='block text-sm font-medium text-gray-700'
                    >
                      Cover photo
                    </label>
                    <div className='mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-md'>
                      <div className='space-y-1 text-center'>
                        <svg
                          className='mx-auto h-12 w-12 text-gray-400'
                          stroke='currentColor'
                          fill='none'
                          viewBox='0 0 48 48'
                          aria-hidden='true'
                        >
                          <path
                            d='M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02'
                            strokeWidth={2}
                            strokeLinecap='round'
                            strokeLinejoin='round'
                          />
                        </svg>
                        <div className='flex text-sm text-gray-600'>
                          <label
                            htmlFor='file-upload'
                            className='relative cursor-pointer bg-white rounded-md font-medium text-primary hover:text-blue-500 focus-within:outline-none focus-within:ring-2 focus-within:ring-offset-2 focus-within:ring-blue-500'
                          >
                            <span>Upload a file</span>
                            <input
                              id='file-upload'
                              name='file-upload'
                              type='file'
                              className='sr-only'
                            />
                          </label>
                          <p className='pl-1'>or drag and drop</p>
                        </div>
                        <p className='text-xs text-gray-500'>
                          PNG, JPG, GIF up to 10MB
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              {/* Personal Information */}
              <div className='pt-8'>
                <div>
                  <h3 className='text-lg leading-6 font-medium text-gray-900'>
                    Personal Information
                  </h3>
                  <p className='mt-1 text-sm text-gray-500'>
                    Use a permanent address where you can receive mail.
                  </p>
                </div>
                <div className='mt-6 grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6'>
                  <div className='sm:col-span-3'>
                    <TextField
                      id='firstName'
                      {...register('firstName', {
                        required: 'This field is required.',
                      })}
                      labelText='First name'
                      isDirty={errors.firstName ? true : false}
                      errorMsg={errors.firstName?.message}
                    />
                  </div>
                  <div className='sm:col-span-3'>
                    <TextField
                      id='lastName'
                      {...register('lastName', {
                        required: 'This field is required.',
                      })}
                      labelText='Last name'
                      isDirty={errors.lastName ? true : false}
                      errorMsg={errors.lastName?.message}
                    />
                  </div>
                  <div className='sm:col-span-3'>
                    <TextField
                      id='email'
                      {...register('email', {
                        required: 'This field is required.',
                      })}
                      labelText='Email'
                      prefix='@example.com'
                      prefixPos='right'
                      isDirty={errors.email ? true : false}
                      errorMsg={errors.email?.message}
                    />
                  </div>
                  <div className='sm:col-span-3'>
                    <Select
                      id='country'
                      labelText='Country'
                      options={countries}
                      {...register('country')}
                    />
                  </div>
                  <div className='sm:col-span-6'>
                    <TextField
                      id='streetAddress'
                      {...register('streetAddress', {
                        required: 'This field is required.',
                      })}
                      labelText='Street address'
                      isDirty={errors.streetAddress ? true : false}
                      errorMsg={errors.streetAddress?.message}
                    />
                  </div>
                  <div className='sm:col-span-2'>
                    <TextField
                      id='city'
                      {...register('city', {
                        required: 'This field is required.',
                      })}
                      labelText='City'
                      isDirty={errors.city ? true : false}
                      errorMsg={errors.city?.message}
                    />
                  </div>
                  <div className='sm:col-span-2'>
                    <TextField
                      id='state'
                      {...register('state', {
                        required: 'This field is required.',
                      })}
                      labelText='State / Province'
                      isDirty={errors.state ? true : false}
                      errorMsg={errors.state?.message}
                    />
                  </div>
                  <div className='sm:col-span-2'>
                    <TextField
                      id='postCode'
                      type={InputTypes.NUMBER}
                      {...register('postCode', {
                        required: 'This field is required.',
                      })}
                      labelText='ZIP / Postal code'
                      isDirty={errors.postCode ? true : false}
                      errorMsg={errors.postCode?.message}
                    />
                  </div>
                </div>
              </div>
              <div className='pt-8'>
                <div>
                  <h3 className='text-lg leading-6 font-medium text-gray-900'>
                    Notifications
                  </h3>
                  <p className='mt-1 text-sm text-gray-500'>
                    We'll always let you know about important changes, but you
                    pick what else you want to hear about.
                  </p>
                </div>
                <div className='mt-6'>
                  <fieldset>
                    <legend className='text-base font-medium text-gray-900'>
                      By Email
                    </legend>
                    <div className='mt-4 space-y-4'>
                      <Checkbox
                        id='comments'
                        labelText='Comments'
                        {...register('comments')}
                        description='Get notified when someones posts a comment on a posting.'
                      />
                      <Checkbox
                        id='candidates'
                        labelText='Candidates'
                        {...register('candidates')}
                        description='Get notified when a candidate applies for a job.'
                      />
                      <Checkbox
                        id='offers'
                        labelText='Offers'
                        {...register('offers')}
                        description='Get notified when a candidate accepts or rejects an offer.'
                      />
                    </div>
                  </fieldset>
                  <fieldset className='mt-6'>
                    <div>
                      <legend className='text-base font-medium text-gray-900'>
                        Push Notifications
                      </legend>
                      <p className='text-sm text-gray-500'>
                        These are delivered via SMS to your mobile phone.
                      </p>
                    </div>
                    {/* TODO: Create Radiobutton UI lib */}
                    <div className='mt-4 space-y-4'>
                      <Radio
                        id='pushEverything'
                        {...register('pushNotifications', {
                          required: 'This field is required.',
                        })}
                        labelText='Everything'
                      />
                      <Radio
                        id='pushEmail'
                        {...register('pushNotifications', {
                          required: 'This field is required.',
                        })}
                        labelText='Same as email'
                      />
                      <Radio
                        id='pushNothing'
                        {...register('pushNotifications', {
                          required: 'This field is required.',
                        })}
                        labelText='No push notifications'
                      />
                    </div>
                  </fieldset>
                </div>
              </div>
            </div>
            <div className='pt-5'>
              <div className='flex justify-end'>
                <Button
                  text='Delete'
                  color={ButtonColors.DANGER}
                  onClick={() => setDeleteModelOpen(true)}
                  className='mr-auto'
                />
                <Button
                  text='Cancel'
                  color={ButtonColors.OUTLINE}
                  onClick={() => navigate(-1)}
                  className='ml-4'
                />
                <Button
                  type={ButtonVariants.SUBMIT}
                  text='Save'
                  className='ml-4'
                />
              </div>
            </div>
          </form>
        </div>
      </MainLayout>

      {deleteModelOpen && (
        <Modal>
          <Transition.Root show={deleteModelOpen} as={Fragment}>
            <Dialog
              as='div'
              className='fixed z-10 inset-0 overflow-y-auto'
              onClose={setDeleteModelOpen}
            >
              <div className='flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0'>
                <Transition.Child
                  as={Fragment}
                  enter='ease-out duration-300'
                  enterFrom='opacity-0'
                  enterTo='opacity-100'
                  leave='ease-in duration-200'
                  leaveFrom='opacity-100'
                  leaveTo='opacity-0'
                >
                  <Dialog.Overlay className='fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity' />
                </Transition.Child>

                {/* This element is to trick the browser into centering the modal contents. */}
                <span
                  className='hidden sm:inline-block sm:align-middle sm:h-screen'
                  aria-hidden='true'
                >
                  &#8203;
                </span>
                <Transition.Child
                  as={Fragment}
                  enter='ease-out duration-300'
                  enterFrom='opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95'
                  enterTo='opacity-100 translate-y-0 sm:scale-100'
                  leave='ease-in duration-200'
                  leaveFrom='opacity-100 translate-y-0 sm:scale-100'
                  leaveTo='opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95'
                >
                  <div className='inline-block align-bottom bg-white rounded-lg px-4 pt-5 pb-4 text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-sm sm:w-full sm:p-6'>
                    <div>
                      <h2 className='text-2xl mb-2 text-center'>
                        Are you sure?
                      </h2>
                    </div>
                    <div className='mt-5 sm:mt-6 flex gap-5'>
                      <button
                        type='button'
                        className='inline-flex justify-center w-full rounded-md border border-transparent shadow-sm px-4 py-2 bg-primary text-base font-medium text-white focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-700 sm:text-sm'
                        onClick={() => setDeleteModelOpen(false)}
                      >
                        Cancel
                      </button>
                      <button
                        type='button'
                        className='inline-flex justify-center w-full rounded-md border border-transparent shadow-sm px-4 py-2 bg-red-600 text-base font-medium text-white hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-700 sm:text-sm'
                        onClick={() => setDeleteModelOpen(false)}
                      >
                        Delete
                      </button>
                    </div>
                  </div>
                </Transition.Child>
              </div>
            </Dialog>
          </Transition.Root>
        </Modal>
      )}
    </>
  );
};
