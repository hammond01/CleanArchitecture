import { ModalForm, ProFormText } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message, Form } from 'antd';
import React, { useEffect } from 'react';
import { categoryApi } from '@/services/api';

interface UpdateCategoryFormProps {
  visible: boolean;
  onCancel: () => void;
  onSuccess: () => void;
  categoryId?: string;
}

const UpdateCategoryForm: React.FC<UpdateCategoryFormProps> = ({ visible, onCancel, onSuccess, categoryId }) => {
  const [messageApi, contextHolder] = message.useMessage();
  const [form] = Form.useForm();

  useEffect(() => {
    let mounted = true;
    if (visible) {
      if (categoryId) {
        categoryApi.getById(String(categoryId)).then((res) => {
          if (!mounted) return;
          if (res && res.statusCode === 200) {
            form.setFieldsValue(res.data);
          } else {
            messageApi.error(res?.message || 'Failed to load category');
          }
        }).catch(() => messageApi.error('Failed to load category'));
      } else {
        form.resetFields();
      }
    }
    return () => { mounted = false; };
  }, [visible, categoryId]);

  return (
    <>
      {contextHolder}
      <ModalForm
        form={form}
        title={<FormattedMessage id="pages.searchTable.edit" defaultMessage="Edit Category" />}
        width="400px"
        visible={visible}
        onVisibleChange={(v) => !v && onCancel()}
        onFinish={async (value) => {
          try {
            let response: any = null;
            if (categoryId) response = await categoryApi.update(String(categoryId), value as any);
            else response = await categoryApi.create(value as any);

            if (response && (response.statusCode === 200 || response.statusCode === 201)) {
              messageApi.success(categoryId ? 'Category updated successfully' : 'Category created successfully');
              onSuccess();
              return true;
            }
            messageApi.error(response?.message || 'Failed to save category');
            return false;
          } catch (_error) {
            messageApi.error('Failed to save category');
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

export default UpdateCategoryForm;
