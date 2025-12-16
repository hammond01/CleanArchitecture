import { ModalForm, ProFormText } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message } from 'antd';
import React from 'react';
import { categoryApi } from '@/services/api';

interface CreateCategoryFormProps {
  visible: boolean;
  onCancel: () => void;
  onSuccess: () => void;
}

const CreateCategoryForm: React.FC<CreateCategoryFormProps> = ({ visible, onCancel, onSuccess }) => {
  const [messageApi, contextHolder] = message.useMessage();

  return (
    <>
      {contextHolder}
      <ModalForm
        title={<FormattedMessage id="pages.searchTable.new" defaultMessage="New Category" />}
        width="400px"
        visible={visible}
        onVisibleChange={(v) => !v && onCancel()}
        onFinish={async (value) => {
          try {
            const response = await categoryApi.create(value as any);
            if (response && response.statusCode === 201) {
              messageApi.success('Category created successfully');
              onSuccess();
              return true;
            }
            messageApi.error(response?.message || 'Failed to create category');
            return false;
          } catch (_error) {
            messageApi.error('Failed to create category');
            return false;
          }
        }}
      >
        <ProFormText
          name="categoryName"
          label={<FormattedMessage id="pages.searchTable.categoryName" defaultMessage="Category Name" />}
          rules={[{ required: true, message: 'Category name is required' }]}
        />
        <ProFormText name="description" label={<FormattedMessage id="pages.searchTable.description" defaultMessage="Description" />} />
      </ModalForm>
    </>
  );
};

export default CreateCategoryForm;
