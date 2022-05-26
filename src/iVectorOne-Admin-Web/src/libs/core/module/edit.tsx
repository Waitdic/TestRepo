import { memo, useEffect, useCallback, FC, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { useForm, SubmitHandler } from 'react-hook-form';
import axios from 'axios';
//
import { RootState } from '@/store';
import { ButtonColors, ButtonVariants, NotificationStatus } from '@/constants';
import { Module } from '@/types';
import { useSlug } from '@/utils/use-slug';
import MainLayout from '@/layouts/Main';
import {
  ErrorBoundary,
  TextField,
  Button,
  Notification,
  Spinner,
} from '@/components';

type Props = {
  error: string | null;
  isLoading: boolean;
};

export const ModuleEdit: FC<Props> = memo(({ error, isLoading }) => {
  const navigate = useNavigate();
  const { slug } = useSlug();

  const modules = useSelector((state: RootState) => state.app.modules);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<Module>();

  const [showNotification, setShowNotification] = useState<boolean>(false);
  const [notification, setNotification] = useState({
    status: NotificationStatus.SUCCESS,
    message: 'Tenant edited successfully.',
  });

  const onSubmit: SubmitHandler<Module> = async (data) => {
    console.log('Form is valid and submitted.', data);

    try {
      const updatedTenant = await axios.patch(
        'http://localhost:3001/module.edit',
        data
      );

      console.log(updatedTenant);
      setNotification({
        status: NotificationStatus.SUCCESS,
        message: 'Tenant edited successfully.',
      });
      setShowNotification(true);
    } catch (error) {
      if (typeof error === 'string') {
        console.log(error.toUpperCase());
        setNotification({
          status: NotificationStatus.ERROR,
          message: error.toUpperCase(),
        });
      } else if (error instanceof Error) {
        console.log(error.message);
        setNotification({
          status: NotificationStatus.ERROR,
          message: error.message,
        });
      }
      setShowNotification(true);
    }
  };

  const loadModule = useCallback(() => {
    if (!modules.length) {
      setNotification({
        status: NotificationStatus.ERROR,
        message: error as string,
      });
      setShowNotification(true);
      return;
    }

    const currentModule = modules.filter(
      (module) => module.moduleId === slug
    )[0];

    if (!currentModule) {
      navigate('/module/list');
    } else {
      setValue('name', currentModule.name);
    }
  }, [setValue, slug, navigate, modules, error]);

  useEffect(() => {
    loadModule();
  }, [loadModule]);

  return (
    <>
      <MainLayout>
        <div className='flex flex-col'>
          {/* Edit Module */}
          {isLoading ? (
            <Spinner />
          ) : typeof error === 'string' ? (
            <ErrorBoundary />
          ) : (
            <>
              <h2 className='md:text-3xl text-2xl font-semibold sm:font-medium text-gray-900 mb-5 pb-3 md:mb-8 md:pb-6'>
                Edit Module
              </h2>
              <form
                className='w-full divide-y divide-gray-200'
                onSubmit={handleSubmit(onSubmit)}
                autoComplete='turnedOff'
              >
                <div className='mb-8 md:w-3/4'>
                  <TextField
                    id='module'
                    {...register('name', {
                      required: 'This field is required.',
                    })}
                    labelText='Name'
                    isDirty={errors.name ? true : false}
                    errorMsg={errors.name?.message}
                  />
                </div>
                <div className='flex justify-end mt-5 pt-5'>
                  <Button
                    text='Cancel'
                    color={ButtonColors.OUTLINE}
                    className='ml-4'
                    onClick={() => navigate(-1)}
                  />
                  <Button
                    type={ButtonVariants.SUBMIT}
                    text='Save'
                    className='ml-4'
                  />
                </div>
              </form>
            </>
          )}
        </div>
      </MainLayout>

      {showNotification && (
        <Notification
          title='Error'
          description={notification.message}
          show={showNotification}
          setShow={setShowNotification}
          status={notification.status}
          autoHide={false}
        />
      )}
    </>
  );
});
